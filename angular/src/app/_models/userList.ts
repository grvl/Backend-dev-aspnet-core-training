import { List } from './list';
import { User } from './user';
export class UserList {
  userId: number;
  listId: number;
  editPermission: boolean;
  list: List;
  user: User;
}
