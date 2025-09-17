import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './common-ui/login/login';
import { RegisterComponent } from "./common-ui/register/register";
import {ForgotPassword} from './common-ui/forgot-password/forgot-password';
import {RegisterDetails} from './common-ui/register-details/register-details';

export const routes: Routes = [
  { path: '', redirectTo: '/register-details', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPassword },
  { path: 'register-details', component: RegisterDetails }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
