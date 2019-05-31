import { Component, OnInit, Input } from '@angular/core';
import { Validators, FormBuilder, FormGroup } from '@angular/forms';
import { ItemService } from 'src/app/_services/item.service';
import { AlertService } from 'src/app/_services';
import { ItemExtended } from 'src/app/_models/itemExtended';
import { Item } from 'src/app/_models/item';
import { first } from 'rxjs/operators';

@Component({
  selector: 'app-item-edit',
  templateUrl: './item-edit.component.html',
  styleUrls: ['./item-edit.component.scss']
})
export class ItemEditComponent implements OnInit {
  @Input() itemExtended:ItemExtended;
  error: string;
  success: string;
  loading = false;
  newItem = false;
  editItemForm: FormGroup;

  constructor(private itemService: ItemService,
            private formBuilder: FormBuilder,
            private alertService: AlertService) { }

  ngOnInit() {
    this.editItemForm = this.formBuilder.group({
        name: ['', Validators.required],
        bought: ['', Validators.required],
        quantity: ['', Validators.required],
        price: ['', Validators.required]
    });

    this.editItemForm.get('name').setValue(this.itemExtended.item.itemName);
    this.editItemForm.get('bought').setValue(this.itemExtended.item.bought);
    this.editItemForm.get('quantity').setValue(this.itemExtended.item.quantity);
    this.editItemForm.get('price').setValue(this.itemExtended.item.price);
  }

  get f() { return this.editItemForm.controls; }

  onSubmit() {

      // stop here if form is invalid
      if (this.editItemForm.invalid) {
          return;
      }

      this.loading = true;
      let item = this.itemExtended.item;
      item.itemName = this.f.name.value;
      item.bought = this.f.bought.value;
      item.quantity = this.f.quantity.value;
      item.price = this.f.price.value;

      this.editItem(item);
  }

  editItem(item: Item){
    // reset alerts on submit
    this.error = null;
    this.success = null;

    this.itemService.editItem(item)
        .pipe(first())
        .subscribe(
            _ => {
                // this.router.navigate();
                this.alertService.success("Item edited successfully", true);
                this.loading = false;
                this.itemExtended.itemEdit = false;
            },
            error => {
                this.alertService.error(error);
                this.loading = false;
            });
  }

  deleteItem(id: number) {
      // reset alerts on submit
      this.error = null;
      this.success = null;

      this.itemService.deleteItem(id)
          .pipe(first())
          .subscribe(
              _ => {
                  // this.router.navigate();
                  location.reload();
              },
              error => {
                  this.alertService.error(error);
                  this.loading = false;
              });
  }

}
