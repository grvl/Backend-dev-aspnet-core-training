import { Component, OnInit, Input } from '@angular/core';
import { List } from 'src/app/_models/list';
import { ActivatedRoute, Router } from '@angular/router';
import { ListService } from 'src/app/_services/list.service';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AlertService } from 'src/app/_services/alert.service';
import { User } from 'src/app/_models/user';
import { Item } from 'src/app/_models/item';
import { ItemExtended } from 'src/app/_models/itemExtended';
import { PaginatedObject } from 'src/app/_models/paginatedObject';
import { ObjectPagination } from 'src/app/_models/ObjectPagination';
import { UserList } from 'src/app/_models/userList';
import { AuthenticationService } from 'src/app/_services';

@Component({
  selector: 'app-list-detail',
  templateUrl: './list-detail.component.html',
  styleUrls: ['./list-detail.component.scss']
})
export class ListDetailComponent implements OnInit {
  @Input() list: List;
  usersWithEditPermission: User[];
  usersWithViewPermission: User[];
  error: string;
  success: string;
  loading = false;
  listForm: FormGroup;
  returnUrl: string;
  listEdit = false;
  jsonString: String;
  items: PaginatedObject<Item>;
  itemsExtended:ItemExtended[];
  listId:number;
  selectedSize = 10;
  sizes = [5, 10, 20];
  newItem = false;
  currentUser:User;
  editPermission = false;

  constructor(
      private route: ActivatedRoute,
      private formBuilder: FormBuilder,
      private listService: ListService,
      private alertService: AlertService,
      private router: Router,
      private authenticationService: AuthenticationService) {
          this.currentUser = this.authenticationService.currentUserValue;
      }

  ngOnInit() {
    this.getList();
    this.listForm = this.formBuilder.group({
        name: ['', Validators.required]
    });
  }

  getList(url?: string,objectPagination?:ObjectPagination) : void {
    this.loading = true;
    const id = +this.route.snapshot.paramMap.get('listId');
    this.listService.getById(id, url, objectPagination
    ).subscribe(
        data => {
            this.list = data.list;
            this.listId = this.list.listId;
            this.items = data.paginatedItems;
            this.getItemsFromArray();
            console.log(this.items);
            this.listForm.get('name').setValue(this.list.listName);
            this.getUsers();
            this.loading = false;
            this.jsonString = JSON.stringify(data);
            this.getEditPermission();
        },
        error => {
            this.alertService.error(error, true);
            this.router.navigate(['/list']);
            this.loading = false;
        });
  }
  getEditPermission(): void {
    let userList = this.list.userList;
    console.log(userList);
    for(let user of userList){
      if(user.userId == this.currentUser.userId){
        this.editPermission = user.editPermission;
        break;
      }
    }
  }

  getUsers(): void {
    this.usersWithEditPermission = new Array();
    this.usersWithViewPermission = new Array();

    for(let userList of this.list.userList){
      if(userList.editPermission){
        this.usersWithEditPermission.push(userList.user);
      }
      else if (!userList.editPermission){
        this.usersWithViewPermission.push(userList.user);
      }
    }
  }

  changeEditForm() : void{
    this.listEdit = !this.listEdit;
  }

  get f() { return this.listForm.controls; }

  onSubmit() {
      // reset alerts on submit
      this.error = null;
      this.success = null;

      // stop here if form is invalid
      if (this.listForm.invalid) {
          return;
      }

      this.loading = true;
      let aux = new List
      aux.listId = this.list.listId;
      aux.listName = this.f.name.value;

      this.listService.update(aux)
          .subscribe(
              _ => {
                this.alertService.success("List edited successfully.");
                this.loading = false;
                this.list.listName = aux.listName;
              },
              error => {
                  this.alertService.error(error);
                  this.loading = false;
              });
  }

  DeleteList() {
    this.listService.delete(this.list.listId)
        .subscribe(
            _ => {
              this.alertService.success('List deleted successfully.', true);
              this.router.navigate(['/list']);
            },
            error => {
                this.alertService.error(error);
                this.loading = false;
            });
  }

  next(){
    this.getList(this.items.next);
  }

  previous(){
    this.getList(this.items.previous);
  }

    getItemsFromArray(): void {
      this.itemsExtended = new Array();
      for (let item of this.items.result){
        let aux = new ItemExtended;
        item.listId = this.listId;
        aux.item = item;
        this.itemsExtended.push(aux);
      }
    }

}
