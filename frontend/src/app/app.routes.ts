import { Routes } from '@angular/router';
import { EmployeeListComponent } from './pages/employee-list/employee-list.component';
import { EmployeeFormComponent } from './pages/employee-form/employee-form.component';
import { LoginComponent } from './pages/login/login.component';
import { EmployeeDetailComponent } from './pages/employee-detail/employee-detail.component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'employees', component: EmployeeListComponent },
    { path: 'employees/new', component: EmployeeFormComponent },
    { path: 'employees/edit/:id', component: EmployeeFormComponent },
    { path: 'employees/:id', component: EmployeeDetailComponent },
];