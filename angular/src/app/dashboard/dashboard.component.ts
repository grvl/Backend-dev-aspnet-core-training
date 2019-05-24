import { Component } from '@angular/core';
import { first } from 'rxjs/operators';

import { User } from '../_models/user';
import { UserService, AuthenticationService } from '../_services';

@Component({templateUrl: 'dashboard.component.html'})
export class DashboardComponent {
    currentUser: User;
    userFromApi: any;

    constructor(
        private userService: UserService,
        private authenticationService: AuthenticationService
    ) {
        this.currentUser = this.authenticationService.currentUserValue;
    }

    ngOnInit() {
        // this.userService.getById(this.currentUser.userId).pipe(first()).subscribe(user => {
        //     this.userFromApi = user;
        // });
        console.log(this.currentUser);
        this.userFromApi = this.currentUser;
    }
}
