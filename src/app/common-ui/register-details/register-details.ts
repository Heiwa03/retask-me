import {Component, inject} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {Router} from '@angular/router';
import {RegisterDetailsService, RegisterDetailsRequest} from '../../data/services/register-details';
import {HttpClient} from '@angular/common/http';

@Component({
  selector: 'app-register-details',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    FormsModule,
    CommonModule
  ],
  templateUrl: './register-details.html',
  styleUrl: './register-details.scss'
})
export class RegisterDetails {
  private registerDetailsService = inject(RegisterDetailsService);
  private router = inject(Router);
  firstName = '';
  lastName = '';
  email = '';
  dob: string | null = null;
  isDobFocused = false;
  gender = '';
  isLoading = false;
  errorMessage = '';
  successMessage = '';

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

  onSubmit() {
    if (!this.firstName || !this.lastName || !this.dob || !this.gender) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const details: RegisterDetailsRequest = {
      firstName: this.firstName,
      lastName: this.lastName,
      dateOfBirth: this.dob,
      gender: this.gender
    };

    this.registerDetailsService.submitRegisterDetails(details).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.successMessage = 'Details submitted successfully!';
          setTimeout(() => {
            this.router.navigateByUrl('/dashboard');
          }, 2000);
        } else {
          this.errorMessage = response.message || 'Failed to submit details';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to submit details. Please try again.';
        console.error('Register details error:', error);
      }
    });
  }
}
