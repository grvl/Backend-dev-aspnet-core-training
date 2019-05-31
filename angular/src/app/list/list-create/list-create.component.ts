import { Component, OnInit } from '@angular/core';
import { ListService } from 'src/app/_services/list.service';
import { List } from 'src/app/_models/list';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';
import { AlertService } from 'src/app/_services';

@Component({
  selector: 'app-list-create',
  templateUrl: './list-create.component.html',
  styleUrls: ['./list-create.component.scss']
})
export class ListCreateComponent implements OnInit {
  listForm: FormGroup;
  loading = false;
  submitted = false;
  returnUrl: string;
  error: string;
  success: string;

  constructor(private listService: ListService,
            private formBuilder: FormBuilder,
            private route: ActivatedRoute,
            private router: Router,
            private alertService: AlertService) { }

  ngOnInit() {
    this.listForm = this.formBuilder.group({
        name: ['', Validators.required]
    });

    this.returnUrl = '/list/';

    // show success message on registration
    if (this.route.snapshot.queryParams['created']) {
        this.success = 'List created successfully.';
    }
  }

   // convenience getter for easy access to form fields
   get f() { return this.listForm.controls; }

   onSubmit() {
       this.submitted = true;

       // reset alerts on submit
       this.error = null;
       this.success = null;

       // stop here if form is invalid
       if (this.listForm.invalid) {
           return;
       }

       this.loading = true;
       let list = new List;
       list.listName = this.f.name.value;
       this.listService.create(list)
           .pipe(first())
           .subscribe(
               data => {
                   this.router.navigate([this.returnUrl + (<List>data).listId]);
               },
               error => {
                   this.alertService.error(error);
                   this.loading = false;
               });
   }
 }
