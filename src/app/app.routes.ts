import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './common-ui/login/login';
import { RegisterComponent } from "./common-ui/register/register";
import {ForgotPassword} from './common-ui/forgot-password/forgot-password';
import {RegisterDetails} from './common-ui/register-details/register-details';
import {Dashboard} from './common-ui/dashboard/dashboard';
import {EmailVerification} from './common-ui/email-verification/email-verification';

export const routes: Routes = [
  { path: '', redirectTo: '/email-verification', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'forgot-password', component: ForgotPassword },
  { path: 'register-details', component: RegisterDetails },
  { path: 'dashboard', component: Dashboard },
  {path:  'email-verification', component : EmailVerification}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
