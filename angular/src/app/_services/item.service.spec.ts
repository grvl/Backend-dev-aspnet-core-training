import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

// Other imports
import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpResponse } from '@angular/common/http';

import { Item } from '../_models/item';
import { ItemService } from './item.service';
import { environment } from '../environments/environment';
import { ObjectPagination } from '../_models/ObjectPagination';

describe('ItemService', () => {
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;
  let itemService: ItemService;
  let usersUrl = `${environment.apiUrl}/item`;

  beforeEach(() => {
    TestBed.configureTestingModule({
      // Import the HttpClient mocking services
      imports: [ HttpClientTestingModule ],
      // Provide the service-under-test and its dependencies
      providers: [
        ItemService
      ]
    });

    // Inject the http, test controller, and service-under-test
    // as they will be referenced by each test.
    httpClient = TestBed.get(HttpClient);
    httpTestingController = TestBed.get(HttpTestingController);
    itemService = TestBed.get(ItemService);
  });

  afterEach(() => {
    // After every test, assert that there are no more pending requests.
    httpTestingController.verify();
  });

  /// Itemservice method tests begin ///

  describe('#getItem Item', () => {
    it('should get an item by id and return it', () => {
      const makeUrl = (id: number) => `${usersUrl}/${id}`;

      const item: Item = { itemId: 1, itemName: "A", listId: 1};

      itemService.getItem(1).subscribe(
        data => expect(data).toEqual( item, 'should return the item'),
        fail
      );

      const req = httpTestingController.expectOne(makeUrl(1));
      expect(req.request.method).toEqual('GET');

      const expectedResponse = new HttpResponse(
        { status: 200, statusText: 'OK', body: item });
      req.event(expectedResponse);
    });
  });

  describe('#editItem Item', () => {
    it('should edit an item and return it', () => {

      const updateItem: Item = { itemId: 1, itemName: "A", listId: 1};

      itemService.editItem(updateItem).subscribe(
        data => expect(data).toEqual( updateItem, 'should return the item'),
        fail
      );

      const req = httpTestingController.expectOne(`${usersUrl}/1`);
      expect(req.request.method).toEqual('PUT');
      expect(req.request.body).toEqual(updateItem);

      const expectedResponse = new HttpResponse(
        { status: 200, statusText: 'OK', body: updateItem });
      req.event(expectedResponse);
    });
  });

  describe('#createItem Item', () => {
    it('should create an item and return it', () => {

      const newItem: Item = { itemId: 0, itemName: "A", listId: 1};

      itemService.createItem(newItem).subscribe(
        data => expect(data).toEqual( newItem, 'should return the item'),
        fail
      );

      const req = httpTestingController.expectOne(`${usersUrl}`);
      expect(req.request.method).toEqual('POST');
      expect(req.request.body).toEqual(newItem);

      const expectedResponse = new HttpResponse(
        { status: 200, statusText: 'OK', body: newItem });
      req.event(expectedResponse);
    });
  });

  describe('#deleteItem Item', () => {
    it('should delete an item and return nothing', () => {

      itemService.deleteItem(1).subscribe(
        _ => expect(null),
        fail
      );

      const req = httpTestingController.expectOne(`${usersUrl}/1`);
      expect(req.request.method).toEqual('DELETE');

      const expectedResponse = new HttpResponse(
        { status: 200, statusText: 'OK' });
      req.event(expectedResponse);
    });
  });
});
