import { List } from './list';
import { PaginatedObject } from './paginatedObject';
export class ListWithPaginatedItems<T> {
  list: List;
  paginatedItems: PaginatedObject<T>;
}
