import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

// Other imports
import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { List } from '../_models/list';
import { environment } from '../environments/environment';
import { ObjectPagination } from '../_models/ObjectPagination';
import { ListWithPaginatedItems } from '../_models/listWithPaginatedItems';
import { PaginatedObject } from '../_models/paginatedObject';
import { ListService } from './list.service';

describe('ListService', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let listService: ListService;
  let listUrl = `${environment.apiUrl}/List`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      // Import the HttpClient mocking services
      imports: [ HttpClientTestingModule ],
      // Provide the service-under-test and its dependencies
      providers: [
        ListService
      ]
    });

    // Inject the http, test controller, and service-under-test
    // as they will be referenced by each test.
    httpClient = TestBed.get(HttpClient);
    httpTestingController = TestBed.get(HttpTestingController);
    listService = TestBed.get(ListService);
  });

  afterEach(() => {
    // After every test, assert that there are no more pending requests.
    httpTestingController.verify();
  });

  /// listService method tests begin ///

  describe('#getAll lists', () => {
      it('should get all lists owned by the user', () => {
        let paginatedObject: PaginatedObject<List> = {total: 1, totalPages: 1, next: "", previous: "", pageSize: 1, pageNumber: 1, result: []};

        listService.getAll().subscribe(
          data => expect(data).toEqual(paginatedObject, 'should return the user'),
          fail
        );

        const req = httpTestingController.expectOne(listUrl);
        expect(req.request.method).toEqual('GET');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: paginatedObject });
        req.event(expectedResponse);
      });

      // it('should get all lists owned by the user and change pages', () => {
      //   let url: string = "List\?page=2&size=10";
      //   let paginatedObject: PaginatedObject<List> = {total: 1, totalPages: 1, next: "", previous: "", pageSize: 1, pageNumber: 1, result: []};
      //
      //   listService.getAll(url).subscribe(
      //     data => expect(data).toEqual(paginatedObject, 'should return the user'),
      //     fail
      //   );
      //
      //   const req = httpTestingController.expectOne(listUrl+"\?page2&size=10");
      //   expect(req.request.method).toEqual('GET');
      //
      //   const expectedResponse = new HttpResponse(
      //     { status: 200, statusText: 'OK', body: paginatedObject });
      //   req.event(expectedResponse);
      // });

      it('should get all lists owned by the user in different sizes', () => {
        let url: string = "";
        let objectPagination: ObjectPagination = {page:1, size: 5}
        let paginatedObject: PaginatedObject<List> = {total: 1, totalPages: 1, next: "", previous: "", pageSize: 1, pageNumber: 1, result: []};

        listService.getAll(url, objectPagination).subscribe(
          data => expect(data).toEqual(paginatedObject, 'should return the user'),
          fail
        );

        const req = httpTestingController.expectOne(listUrl+"\?size=5&page=1");
        expect(req.request.method).toEqual('GET');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: paginatedObject });
        req.event(expectedResponse);
      });
    });

    describe('#getById List', () => {
      it('should get a list by id and return it', () => {
        const makeUrl = (id: number) => `${listUrl}/${id}`;

        const list: List = { listId: 1, listName: "A" };
        const listWithPagintedItems: ListWithPaginatedItems = {list: new List, paginatedItems: null};

        listService.getById(1).subscribe(
          data => expect(data).toEqual( listWithPagintedItems, 'should return the list'),
          fail
        );

        const req = httpTestingController.expectOne(makeUrl(1));
        expect(req.request.method).toEqual('GET');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: listWithPagintedItems });
        req.event(expectedResponse);
      });
    });

    describe('#create List', () => {
      it('should create a list and return it', () => {

        const newList: List ={ listId: 1, listName: "A" };

        listService.create(newList).subscribe(
          data => expect(data).toEqual( newList, 'should return the new list'),
          fail
        );

        const req = httpTestingController.expectOne(listUrl);
        expect(req.request.method).toEqual('POST');
        expect(req.request.body).toEqual(newList);

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: newList });
        req.event(expectedResponse);
      });
    });

    describe('#edit List', () => {
      it('should edit a list and return it', () => {

        const newList: List ={ listId: 1, listName: "A" };

        listService.update(newList).subscribe(
          data => expect(data).toEqual( newList, 'should return the new list'),
          fail
        );

        const req = httpTestingController.expectOne(listUrl+"/1");
        expect(req.request.method).toEqual('PUT');
        expect(req.request.body).toEqual(newList);

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: newList });
        req.event(expectedResponse);
      });
    });

    describe('#delete List', () => {
      it('should delete a list and return nothing', () => {

        listService.delete(1).subscribe(
          _ => _,
          fail
        );

        const req = httpTestingController.expectOne(listUrl+"/1");
        expect(req.request.method).toEqual('DELETE');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK' });
        req.event(expectedResponse);
      });
    });

    describe('#share List', () => {
      it('should share a list with another user and return it', () => {

        const newList: List ={ listId: 1, listName: "A" };

        listService.share(1, 1).subscribe(
          data => expect(data).toEqual( newList, 'should return the new list'),
          fail
        );

        const req = httpTestingController.expectOne(listUrl+"/1/1");
        expect(req.request.method).toEqual('GET');

        const expectedResponse = new HttpResponse(
          { status: 200, statusText: 'OK', body: newList });
        req.event(expectedResponse);
      });
    });
});
