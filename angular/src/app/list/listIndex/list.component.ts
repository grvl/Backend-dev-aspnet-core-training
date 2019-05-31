import { Component, OnInit } from '@angular/core';
import { ListService } from '../../_services/list.service';
import { PaginatedObject } from '../../_models/paginatedObject';
import { List } from '../../_models/list';
import { AlertService } from 'src/app/_services';
import { ObjectPagination } from 'src/app/_models/ObjectPagination';

@Component({
  selector: 'app-list',
  templateUrl: './list.component.html',
  styleUrls: ['./list.component.scss']
})
export class ListComponent implements OnInit {
  lists: PaginatedObject<List>;
  error: string;
  success: string;
  loading = false;
  selectedSize = 10;
  sizes = [5, 10, 20];

  constructor(private listService: ListService,
              private alertService: AlertService) { }

  ngOnInit() {
    this.getLists();
  }

  getLists(url?:string, objectPagination?:ObjectPagination) : void {

    this.listService.getAll(url, objectPagination)
    .subscribe(
        lists => this.lists = lists,
        error => {
            this.alertService.error(error);
            this.loading = false;
        });
  }

  next(){
    this.getLists(this.lists.next);
    console.log(this.lists)
    console.log(this.selectedSize);
  }

  previous(){
    this.getLists(this.lists.previous);
  }

  onSizeSelect(size:number) {
    let objectPagination = new ObjectPagination;
    objectPagination.size = size;
    objectPagination.page = 1;
    this.getLists("", objectPagination);
  }

}
