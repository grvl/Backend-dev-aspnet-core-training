import { List } from './list';
export class Item {
  itemId: number;
  listId: number;
  itemName: string;
  quantity: number;
  price: number;
  bought: boolean;
  list: List;
}
