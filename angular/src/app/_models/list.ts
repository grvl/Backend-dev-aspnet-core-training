import { UserList } from './userList';
import { Item } from './item';
export class List {
  listId: number;
  listName: String;
  itemList?: Item;
  userList?: UserList[];
}
