import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

// Other imports
import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { User } from '../_models/user';
import { UserService } from './user.service';
import { environment } from '../environments/environment';
import { ObjectPagination } from '../_models/ObjectPagination';
import { PaginatedObject } from '../_models/paginatedObject';

describe('UserService', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let userService: UserService;
  let usersUrl = `${environment.apiUrl}/Users`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      // Import the HttpClient mocking services
      imports: [ HttpClientTestingModule ],
      // Provide the service-under-test and its dependencies
      providers: [
        UserService
      ]
    });

    // Inject the http, test controller, and service-under-test
    // as they will be referenced by each test.
    httpClient = TestBed.get(HttpClient);
    httpTestingController = TestBed.get(HttpTestingController);
    userService = TestBed.get(UserService);
  });

  afterEach(() => {
    // After every test, assert that there are no more pending requests.
    httpTestingController.verify();
  });

  /// Userservice method tests begin ///

  describe('#getAll Users', () => {
    let expectedUsers: User[];

    beforeEach(() => {
      userService = TestBed.get(UserService);
      expectedUsers= [
        { userId: 1, username: 'A' },
        { userId: 2, username: 'B' },
      ] as User[];
    });

    it('should return expected users (called once)', () => {

      userService.getAll().subscribe(
        users => expect(users).toEqual(expectedUsers, 'should return expected users'),
        fail
      );

      const req = httpTestingController.expectOne(usersUrl);
      expect(req.request.method).toEqual('GET');

      req.flush(expectedUsers);
    });

    it('should return expected users (called multiple times)', () => {

      userService.getAll().subscribe();
      userService.getAll().subscribe();
      userService.getAll().subscribe(
        users => expect(users).toEqual(expectedUsers, 'should return expected users'),
        fail
      );

      const requests = httpTestingController.match(usersUrl);
      expect(requests.length).toEqual(3, 'calls to getAll()');

      // Respond to each request with different mock user results
      requests[0].flush([]);
      requests[1].flush([{userId: 1, username: 'bob'}]);
      requests[2].flush(expectedUsers);
    });
  });

  describe('#getById User', () => {
    it('should get an user by id and return it', () => {
      const makeUrl = (id: number) => `${usersUrl}/${id}`;

      const user: User = { userId: 1, username: 'A', pswd: null };

      userService.getById(1).subscribe(
        data => expect(data).toEqual( user, 'should return the user'),
        fail
      );

      const req = httpTestingController.expectOne(makeUrl(1));
      expect(req.request.method).toEqual('GET');

      const expectedResponse = new HttpResponse(
        { status: 200, statusText: 'OK', body: user });
      req.event(expectedResponse);
    });
  });

  describe('#register User', () => {
    it('should register an user and return it', () => {

      const registerUser: User = { userId: 1, username: 'A', pswd: "B" };

      userService.register(registerUser).subscribe(
        data => expect(data).toEqual( { userId: 1, username: 'A' }, 'should return the user without password'),
        fail
      );

      const req = httpTestingController.expectOne(`${usersUrl}/register`);
      expect(req.request.method).toEqual('POST');
      expect(req.request.body).toEqual(registerUser);

      const expectedResponse = new HttpResponse(
        { status: 200, statusText: 'OK', body: { userId: 1, username: 'A' } });
      req.event(expectedResponse);
    });
  });

  describe('#Search users', () => {
      // Expecting the query form of URL so should not 404 when id not found
      const makeUrl = (searchQuery: string) => `${usersUrl}/search?query=${searchQuery}`;

      it('should search for users by username', () => {

        const searchUser: User = { userId: 1, username: 'Aa', pswd: null };
        const paginatedObject: PaginatedObject<User> = {result: [searchUser], total: 1, totalPages: 1, next: "", previous: "", pageSize: 10, pageNumber: 1}

        userService.search("A").subscribe(
          data => expect(data).toEqual(paginatedObject, 'should return the user'),
          fail
        );

        const req = httpTestingController.expectOne(makeUrl("A"));
        expect(req.request.method).toEqual('GET');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: paginatedObject });
        req.event(expectedResponse);
      });

      it('should be able to return a second page', () => {

        const searchUser: User = { userId: 1, username: 'Aa', pswd: null };
        const paginatedObject: PaginatedObject<User> = {result: [searchUser], total: 1, totalPages: 1, next: "", previous: "", pageSize: 10, pageNumber: 1}

        userService.search("A", "Users/search\?query=A&size=10&page=2").subscribe(
          data => expect(data).toEqual(paginatedObject, 'should return the user'),
          fail
        );

        const req = httpTestingController.expectOne(makeUrl("A&size=10&page=2"));
        expect(req.request.method).toEqual('GET');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: paginatedObject });
        req.event(expectedResponse);
      });

      it('should be able to return a different sized page', () => {

        const searchUser: User = { userId: 1, username: 'Aa', pswd: null };
        let objectPagination: ObjectPagination = {page: 1, size: 5};
        const paginatedObject: PaginatedObject<User> = {result: [searchUser], total: 1, totalPages: 1, next: "", previous: "", pageSize: 10, pageNumber: 1}

        userService.search("A", "", objectPagination).subscribe(
          data => expect(data).toEqual(paginatedObject, 'should return the user'),
          fail
        );

        const req = httpTestingController.expectOne(makeUrl("A&size=5&page=1"));
        expect(req.request.method).toEqual('GET');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: paginatedObject });
        req.event(expectedResponse);
      });

    });
});
