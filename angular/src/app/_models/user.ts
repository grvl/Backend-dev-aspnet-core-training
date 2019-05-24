import { UserList } from './userList';

export class User {
  userId: number;
  username: string;
  pswd: string;
  userRole?: string;
  token?: string;
  userlist?: UserList;
}
