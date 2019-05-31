import { Component, OnInit } from '@angular/core';

import { User } from '../_models/user';
import { UserService, AlertService } from '../_services';

@Component({templateUrl: 'admin.component.html'})
export class AdminComponent implements OnInit {
    users: User[] = [];

    constructor(
        private userService: UserService,
        private alertService: AlertService) {}

    ngOnInit() {
        this.userService.getAll().subscribe(
          users => {
            this.users = users;
          },
          error => {
            this.alertService.error(error, true);
          });
    }
}
