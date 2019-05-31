import { Component, OnInit, Input } from '@angular/core';
import { User } from '../_models/user';
import { UserService, AlertService } from '../_services';
import { PaginatedObject } from '../_models/paginatedObject';
import { ObjectPagination } from '../_models/ObjectPagination';
import { ListService } from '../_services/list.service';

@Component({
  selector: 'app-user-search',
  templateUrl: './user-search.component.html',
  styleUrls: ['./user-search.component.scss']
})
export class UserSearchComponent implements OnInit {
  @Input() listId: number;
  paginatedUsers: PaginatedObject<User>;
  loading = false;
  term: string;

  constructor(private userservice: UserService,
    private alertService: AlertService,
    private listService: ListService) {}

  search(term: string, url?: string, objectPagination?: ObjectPagination):void {
    this.loading = true;
    this.userservice.search(term, url, objectPagination).subscribe(
        data => {
            console.log(data);
            this.paginatedUsers = data;
            this.term = term;
            this.loading = false;
        },
        error => {
            this.alertService.error(error, true);
            this.loading = false;
        });
  }

  ngOnInit() {
  }

  next(){
    this.search(this.term, this.paginatedUsers.next);
  }

  previous(){
    this.search(this.term, this.paginatedUsers.previous);
  }

  onSizeSelect(size:number) {
    let objectPagination = new ObjectPagination;
    objectPagination.size = size;
    objectPagination.page = 1;
    this.search("","", objectPagination);
  }

  share(userId:number){
    this.listService.share(this.listId, userId).subscribe(
        _ => {
            this.alertService.success("List shared successfully.", true);
            location.reload();
        },
        error => {
            this.alertService.error(error);
            this.loading = false;
        });
  }

}
