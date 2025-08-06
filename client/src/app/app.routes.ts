import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { RegisterComponent } from './auth/register/register.component';
import { ManagerDashboardComponent } from './manager-dashboard/manager-dashboard.component'
import { TechnicianDashboardComponent } from './technician-dashboard/technician-dashboard.component';

export const routes: Routes = [
    { path: '', redirectTo: 'login', pathMatch: 'full' },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    { path: 'manager-dashboard', component: ManagerDashboardComponent },
    { path: 'technician-dashboard', component: TechnicianDashboardComponent},

];
