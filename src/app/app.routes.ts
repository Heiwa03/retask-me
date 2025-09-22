import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './common-ui/login/login';
import { Register } from "./common-ui/register/register";
import {ForgotPassword} from './common-ui/forgot-password/forgot-password';
import {RegisterDetails} from './common-ui/register-details/register-details';

export const routes: Routes = [
  { path: '', redirectTo: '/register-details', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'forgot-password', component: ForgotPassword },
  { path: 'register-details', component: RegisterDetails }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
