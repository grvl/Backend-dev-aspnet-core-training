import { async, TestBed } from '@angular/core/testing';

import { AdminComponent } from './admin.component';
import { UserService, AlertService } from '../_services';
import { HttpClient } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { User } from '../_models/user';
import { of } from 'rxjs';

describe('AdminComponent', () => {
  let component: AdminComponent;
  let userService: UserService;
  let alertService: AlertService;
  let httpClient: HttpClient;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      // Import the HttpClient mocking services
      imports: [ HttpClientTestingModule ]
    });
    httpClient = TestBed.get(HttpClient);
    userService = new UserService(httpClient);
    component = new AdminComponent(userService, alertService);
  }));

  afterEach(() => {
        userService = null;
        alertService = null;
        component = null;
    });

    describe('#getAll Users', () => {

      it('should grab a list of all users', () => {
        spyOn(userService, 'getAll').and.returnValue(
          of([new User])
        );
        component.ngOnInit();
        expect(userService.getAll).toHaveBeenCalled();
      });
    });
});
