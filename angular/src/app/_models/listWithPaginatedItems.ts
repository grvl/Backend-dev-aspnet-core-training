import { List } from './list';
import { PaginatedObject } from './paginatedObject';
import { Item } from './item';
export class ListWithPaginatedItems {
  list: List;
  paginatedItems: PaginatedObject<Item>;
}
