import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import {Login} from './common-ui/login/login';
import {RegisterDetails} from './common-ui/register-details/register-details';
// import {ForgotPassword} from './common-ui/forgot-password/';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Login, RegisterDetails],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('ReTask_Me');
}
