import { Component } from '@angular/core';
import {ReactiveFormsModule} from '@angular/forms';

@Component({
  selector: 'app-register-details',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './register-details.html',
  styleUrl: './register-details.scss'
})
export class RegisterDetails {
  form: any;

  submit() {

  }

  openDatePicker(dobEl: HTMLInputElement) {

  }
}
