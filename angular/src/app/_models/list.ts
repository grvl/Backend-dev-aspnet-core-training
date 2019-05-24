import { UserList } from './userList';
import { User } from './user';
export class List {
  userId: number;
  listId: number;
  editPermission: boolean;
  list: List;
  user: User;
  userList: UserList;
}
