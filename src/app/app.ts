import {Component, inject, signal} from '@angular/core';
import { RouterOutlet } from '@angular/router';
// Standalone pages are routed; no need to import here
import {ForgotPasswordService} from './data/services/forgot-password';
import {JsonPipe} from '@angular/common';
// import {ForgotPassword} from './common-ui/forgot-password/';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, JsonPipe],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('ReTask_Me');

  forgotPassService = inject(ForgotPasswordService); //instanta a service-ului
  resultForgotPass: any = null;
  theme?: string;

  constructor() {
    this.forgotPassService.getTestForgotPassword()
      .subscribe(val => {
        this.resultForgotPass = val;
        console.log('forgot-password test:', val);
      });
  }

  toggleTheme() {

  }
}
