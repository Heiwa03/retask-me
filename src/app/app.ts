import {Component, inject, signal} from '@angular/core';
import {RouterOutlet } from '@angular/router';
import {Login} from './common-ui/login/login';
import {RegisterDetails} from './common-ui/register-details/register-details';
import {ForgotPasswordService} from './data/services/forgot-password';
import {JsonPipe} from '@angular/common';
import {Dashboard} from './common-ui/dashboard/dashboard';
// import {ForgotPassword} from './common-ui/forgot-password/';
import { HttpClientModule } from '@angular/common/http';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RegisterDetails, JsonPipe, Dashboard, HttpClientModule],
  templateUrl: './app.html',
  styleUrls: ['./app.scss']
})
export class App {
  protected readonly title = signal('ReTask_Me');

  forgotPassService = inject(ForgotPasswordService);
  resultForgotPass: any;
  theme?: string;

  constructor() {
    this.forgotPassService.getTestForgotPassword()
      .subscribe(val => {
        this.resultForgotPass = val;
        console.log('forgot-password test:', val);
      });
   }

  toggleTheme() {
    // Theme toggle functionality can be implemented here if needed
  }
}
