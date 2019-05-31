import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { environment } from '../environments/environment';
import { User } from '../_models/user';
import { ObjectPagination } from '../_models/ObjectPagination';
import { Observable } from 'rxjs';
import { PaginatedObject } from '../_models/paginatedObject';

@Injectable({ providedIn: 'root' })
export class UserService {
    constructor(private http: HttpClient) { }

    getAll() {
        return this.http.get<User[]>(`${environment.apiUrl}/Users`);
    }

    getById(id: number) {
        return this.http.get(`${environment.apiUrl}/Users/${id}`);
    }

    register(user: User) {
        return this.http.post(`${environment.apiUrl}/Users/register`, user);
    }

    search(searchQuery: string, extraUrl?: string, objectPagination?: ObjectPagination):Observable<PaginatedObject<User>> {
        if (extraUrl){
          return this.http.get<PaginatedObject<User>>(`${environment.apiUrl}/` + extraUrl);
        }
        let url = `${environment.apiUrl}/Users/search?query=${searchQuery}`;
        if(objectPagination){
          return this.http.get<PaginatedObject<User>>(url + `&size=${objectPagination.size}&page=${objectPagination.page}`);
        }
        return this.http.get<PaginatedObject<User>>(url);
    }
}
