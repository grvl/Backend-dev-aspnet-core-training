import { Component, OnInit, Input } from '@angular/core';
import { Item } from 'src/app/_models/item';
import { FormGroup} from '@angular/forms';
import { ItemExtended } from 'src/app/_models/itemExtended';
import { PaginatedObject } from 'src/app/_models/paginatedObject';

@Component({
  selector: 'app-item',
  templateUrl: './item.component.html',
  styleUrls: ['./item.component.scss']
})
export class ItemComponent implements OnInit {
  @Input() paginatedItems:PaginatedObject<Item>;
  @Input() listId:number;
  itemsExtended:ItemExtended[];
  error: string;
  success: string;
  loading = false;
  newItem = false;
  editItemForm: FormGroup;

  constructor() {}

  ngOnInit() {
    this.getItemsFromArray();
  }

  getItemsFromArray(): void {
    this.itemsExtended = new Array();
    for (let item of this.paginatedItems.result){
      let aux = new ItemExtended;
      item.listId = this.listId;
      aux.item = item;
      this.itemsExtended.push(aux);
    }
  }
}
