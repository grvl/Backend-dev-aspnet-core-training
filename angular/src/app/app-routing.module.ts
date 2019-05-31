import { NgModule }             from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { DashboardComponent }   from './dashboard/dashboard.component';
import { ListComponent }      from './list/listIndex/list.component';
import { ItemComponent }  from './item/item-details/item.component';
import { LoginComponent } from './login/login.component';
import { ListDetailComponent } from './list/list-detail/list-detail.component';
import { ListCreateComponent } from './list/list-create/list-create.component'
import { RegisterComponent } from './register/register.component';
import { AuthGuard } from './_guards/auth.guard';
import { AdminComponent } from './admin/admin.component';
import { Role } from './_models/role';


const routes: Routes = [
  { path: '', redirectTo: '/dashboard', pathMatch: 'full', canActivate: [AuthGuard] },
  {path: 'login', component: LoginComponent},
  {path: 'register', component: RegisterComponent},
  { path: 'dashboard', component: DashboardComponent, canActivate: [AuthGuard] },
  { path: 'list', component: ListComponent, canActivate: [AuthGuard] },
  { path: 'list/new', component: ListCreateComponent, canActivate: [AuthGuard] },
  { path: 'list/:listId', component: ListDetailComponent, canActivate: [AuthGuard] },
  { path: 'list/:listId/:itemId', component: ItemComponent, canActivate: [AuthGuard] },
  {path: 'admin', component: AdminComponent, canActivate: [AuthGuard], data: { userRoles: [Role.Admin] }
  },

  { path: '**', redirectTo: '' }
];

@NgModule({
  imports: [ RouterModule.forRoot(routes) ],
  exports: [ RouterModule ]
})
export class AppRoutingModule {}
