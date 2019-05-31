import { async, TestBed } from '@angular/core/testing';

import { ItemCreateComponent } from './item-create.component';
import { AlertService } from '../../_services';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { FormBuilder } from '@angular/forms';
import { ItemService } from 'src/app/_services/item.service';
import { Item } from 'src/app/_models/item';

describe('ItemCreateComponent', () => {
  let component: ItemCreateComponent;
  let itemService: ItemService;
  let alertService: AlertService;
  let httpClient: HttpClient;
  let formBuilder: FormBuilder;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      // Import the HttpClient mocking services
      imports: [ HttpClientTestingModule ],
      providers: [FormBuilder]
    });
    httpClient = TestBed.get(HttpClient);
    itemService = new ItemService(httpClient);

    component = new ItemCreateComponent(itemService, formBuilder, alertService);
  }));

  afterEach(() => {
        itemService = null;
        alertService = null;
        component = null;
    });

    describe('#CreateItem Item', () => {

      it('should create an item', () => {
        spyOn(itemService, 'createItem').and.returnValue(
          of([new Item])
        );
        component.createItem(new Item);
        expect(itemService.createItem).toHaveBeenCalled();
      });
    });
});
