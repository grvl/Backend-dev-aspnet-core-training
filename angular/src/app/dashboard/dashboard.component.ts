import { Component } from '@angular/core';
import { first } from 'rxjs/operators';

import { User } from '../_models/user';
import { UserService, AuthenticationService } from '../_services';

@Component({templateUrl: 'dashboard.component.html'})
export class DashboardComponent {
    currentUser: User;

    constructor(
        private authenticationService: AuthenticationService
    ) {
        this.currentUser = this.authenticationService.currentUserValue;
    }

    ngOnInit() {
    }
}
