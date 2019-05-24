import { Injectable } from '@angular/core';
import { HttpRequest, HttpResponse, HttpHandler, HttpEvent, HttpInterceptor, HTTP_INTERCEPTORS } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { delay, mergeMap, materialize, dematerialize } from 'rxjs/operators';
import { User } from '../_models/user';
import { Role } from '../_models/role';

@Injectable()
export class FakeBackendInterceptor implements HttpInterceptor {

    constructor() { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        // array in local storage for registered users
        const users: User[] = [
             { userId: 1, username: 'admin', pswd: 'admin', userRole: Role.Admin, token: null, userlist: null},
             { userId: 2, username: 'user', pswd: 'user', userRole: Role.User, token: null, userlist: null},
         ];

         const authHeader = request.headers.get('Authorization');
         const isLoggedIn = authHeader && authHeader.startsWith('Bearer fake-jwt-token');
         const roleString = isLoggedIn && authHeader.split('.')[1];
         const role = roleString ? Role[roleString] : null;

         // wrap in delayed observable to simulate server api call
         return of(null).pipe(mergeMap(() => {

             // authenticate - public
             if (request.url.endsWith('/users/authenticate') && request.method === 'POST') {
                 const user = users.find(x => x.username === request.body.username && x.pswd === request.body.password);
                 if (!user) return error('Username or password is incorrect');
                 return ok({
                     userId: user.userId,
                     username: user.username,
                     userRole: user.userRole,
                     token: `fake-jwt-token.${user.userRole}`
                 });
             }

             // get user by id - admin or user (user can only access their own record)
             if (request.url.match(/\/users\/\d+$/) && request.method === 'GET') {
                 if (!isLoggedIn) return unauthorised();

                 // get id from request url
                 let urlParts = request.url.split('/');
                 let id = parseInt(urlParts[urlParts.length - 1]);

                 // only allow normal users access to their own record
                 const currentUser = users.find(x => x.userRole === role);
                 if (id !== currentUser.userId && role !== Role.Admin) return unauthorised();

                 const user = users.find(x => x.userId === id);
                 return ok(user);
             }

             // get all users (admin only)
             if (request.url.endsWith('/users') && request.method === 'GET') {
                 if (role !== Role.Admin) return unauthorised();
                 return ok(users);
             }

             // pass through any requests not handled above
             return next.handle(request);
         }))
         // call materialize and dematerialize to ensure delay even if an error is thrown (https://github.com/Reactive-Extensions/RxJS/issues/648)
         .pipe(materialize())
         .pipe(delay(500))
         .pipe(dematerialize());

         // private helper functions

         function ok(body) {
             return of(new HttpResponse({ status: 200, body }));
         }

         function unauthorised() {
             return throwError({ status: 401, error: { message: 'Unauthorised' } });
         }

         function error(message) {
             return throwError({ status: 400, error: { message } });
         }
     }
 }

 export let fakeBackendProvider = {
     // use fake backend in place of Http service for backend-less development
     provide: HTTP_INTERCEPTORS,
     useClass: FakeBackendInterceptor,
     multi: true
 };
