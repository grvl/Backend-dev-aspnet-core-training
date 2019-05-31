import { async, TestBed } from '@angular/core/testing';

import { ItemEditComponent } from './item-edit.component';
import { AlertService } from '../../_services';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { FormBuilder } from '@angular/forms';
import { ItemService } from 'src/app/_services/item.service';
import { Item } from 'src/app/_models/item';

describe('ItemEditComponent', () => {
  let component: ItemEditComponent;
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

    component = new ItemEditComponent(itemService, formBuilder, alertService);
  }));

  afterEach(() => {
        itemService = null;
        alertService = null;
        component = null;
    });

    describe('#EditItem Item', () => {

      it('should edit an item', () => {
        spyOn(itemService, 'editItem').and.returnValue(
          of([new Item])
        );
        component.editItem(new Item);
        expect(itemService.editItem).toHaveBeenCalled();
      });
    });

    // describe('#deleteItem Item', () => {
    //
    //   it('should delete an item', () => {
    //     spyOn(itemService, 'deleteItem').and.returnValue(of(null));
    //
    //     component.deleteItem(1);
    //     expect(itemService.deleteItem).toHaveBeenCalled();
    //   });
});
