import {Component, inject} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-register-details',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule
  ],
  templateUrl: './register-details.html',
  styleUrl: './register-details.scss'
})
export class RegisterDetails {
  http = inject(HttpClient);
  firstName = '';
  lastName = '';
  email = '';
  dob: string | null = null;
  isDobFocused = false;
  gender = '';

  openDobPicker(input: HTMLInputElement): void {
    const anyInput = input as any;
    if (typeof anyInput.showPicker === 'function') {
      anyInput.showPicker();
    } else {
      input.focus();
      input.click();
    }
  }

  ngOnInit() {
    const state = history.state || {};
    this.firstName = state.firstName || '';
    this.lastName = state.lastName || '';
    this.email = state.email || '';
    const saved = localStorage.getItem('theme') as 'light' | 'dark' | null;
    const dark = saved ? saved === 'dark' : false; // default LIGHT
    document.documentElement.classList.toggle('theme-dark', dark);
    queueMicrotask(() => {
      const chk = document.getElementById('themeSwitch') as HTMLInputElement | null;
      if (chk) chk.checked = !dark; // checked = Light
    });
  }

  onThemeToggle(e: Event) {
    const isLight = (e.target as HTMLInputElement).checked;
    document.documentElement.classList.toggle('theme-dark', !isLight);
    localStorage.setItem('theme', isLight ? 'light' : 'dark');
  }
}
