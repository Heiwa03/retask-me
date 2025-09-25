import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { ForgotPasswordService, ForgotPasswordRequest } from '../../data/services/forgot-password';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss'
})
export class ForgotPassword {
  private forgotPasswordService = inject(ForgotPasswordService);
  private router = inject(Router);
  email = new FormControl('');
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  ngOnInit() {
    const saved = localStorage.getItem('theme') as 'light' | 'dark';
    const dark = saved ? saved === 'dark' : false; // default LIGHT
    document.documentElement.classList.toggle('theme-dark', dark);
    queueMicrotask(() => {
      const chk = document.getElementById('fpThemeSwitch') as HTMLInputElement;
      if (chk) chk.checked = !dark; // checked = Light
    });
  }

  onThemeToggle(e: Event) {
    const isLight = (e.target as HTMLInputElement).checked;
    document.documentElement.classList.toggle('theme-dark', !isLight);
    localStorage.setItem('theme', isLight ? 'light' : 'dark');
  }

  send() {
    if (!this.email.value) {
      this.errorMessage = 'Please enter your email address';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.successMessage = '';

    const request: ForgotPasswordRequest = {
      email: this.email.value
    };

    this.forgotPasswordService.sendResetEmail(request).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.successMessage = 'Password reset email sent successfully!';
          setTimeout(() => {
            this.router.navigateByUrl('/login');
          }, 2000);
        } else {
          this.errorMessage = response.message || 'Failed to send reset email';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Failed to send reset email. Please try again.';
        console.error('Forgot password error:', error);
      }
    });
  }

  close() {
    this.router.navigateByUrl('/login');
  }
}
