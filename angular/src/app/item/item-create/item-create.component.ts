import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AlertService } from 'src/app/_services';
import { Item } from 'src/app/_models/item';
import { first } from 'rxjs/operators';
import { ItemService } from 'src/app/_services/item.service';

@Component({
  selector: 'app-item-create',
  templateUrl: './item-create.component.html',
  styleUrls: ['./item-create.component.scss']
})
export class ItemCreateComponent implements OnInit {
  @Input() listId:number;
  newItemForm: FormGroup;
  loading = false;
  error: string;
  success: string;

  constructor(private itemService: ItemService,
            private formBuilder: FormBuilder,
            private alertService: AlertService) { }

  ngOnInit() {
    this.newItemForm = this.formBuilder.group({
        name: ['', Validators.required],
        bought: ['', Validators.required],
        quantity: ['', Validators.required],
        price: ['', Validators.required]
    });
  }

   // convenience getter for easy access to form fields
   get f() { return this.newItemForm.controls; }

   onSubmit() {

       // stop here if form is invalid
       if (this.newItemForm.invalid) {
           return;
       }

       this.loading = true;
       let item = new Item;
       item.itemName = this.f.name.value;
       item.bought = this.f.bought.value;
       item.quantity = this.f.quantity.value;
       item.price = this.f.price.value;
       item.listId = this.listId;
       this.createItem(item);
   }

   createItem(item: Item){
     // reset alerts on submit
     this.error = null;
     this.success = null;

     this.itemService.createItem(item)
         .pipe(first())
         .subscribe(
             _ => {
                 // this.router.navigate();
                 this.alertService.success("Item created successfully", true);
                 this.loading = false;
                 location.reload();
             },
             error => {
                 this.alertService.error(error);
                 this.loading = false;
             });
   }
 }
