import { ItemComponent } from './item.component';
import { Item } from 'src/app/_models/item';

describe('ItemComponent', () => {
  let component: ItemComponent;

  beforeEach(() => {
    component = new ItemComponent();
  });

  afterEach(() => {
    });

    describe('#getItemsFromArray', () => {

      it('should grab paginatedItems and make them Extended', () => {
        let item: Item = new Item;
        item.listId = 0;
        component.paginatedItems = {total: 1, totalPages: 1, next: "", previous: "", pageSize: 1, pageNumber: 1, result: [item]};

        component.listId = 1;

        component.getItemsFromArray();

        item.listId = 1;

        expect(component.itemsExtended[0].item.listId).toEqual(1);
      });
    });
});
