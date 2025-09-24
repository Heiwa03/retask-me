import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { Login } from './common-ui/login/login';
import { Register } from "./common-ui/register/register";
import {ForgotPassword} from './common-ui/forgot-password/forgot-password';
import {RegisterDetails} from './common-ui/register-details/register-details';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from './data/services/auth.service';
import { Dashboard } from './common-ui/dashboard/dashboard';
import { RootGate } from './common-ui/root-gate/root-gate';
import { NotFound } from './common-ui/not-found/not-found';
import { ApiDocsComponent } from './common-ui/api-docs/api-docs';

const AuthGuard = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (auth.isAuthenticated()) return true;
  router.navigateByUrl('/login');
  return false;
};

const GuestGuard = () => {
  const auth = inject(AuthService);
  const router = inject(Router);
  if (!auth.isAuthenticated()) return true;
  router.navigateByUrl('/dashboard');
  return false;
};

export const routes: Routes = [
  { path: '', component: RootGate, pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [GuestGuard] },
  { path: 'register', component: Register, canActivate: [GuestGuard] },
  { path: 'forgot-password', component: ForgotPassword, canActivate: [GuestGuard] },
  { path: 'register-details', component: RegisterDetails, canActivate: [GuestGuard] },
  { path: 'dashboard', component: Dashboard, canActivate: [AuthGuard] },
  { path: 'api-docs', component: ApiDocsComponent },
  { path: '**', component: NotFound }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
