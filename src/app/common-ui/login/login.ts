import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { inject } from '@angular/core';
import { AuthService, LoginRequest } from '../../data/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterModule, CommonModule, FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class Login {
  email = '';
  password = '';
  isLoading = false;
  errorMessage = '';
  private auth = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    const saved = localStorage.getItem('theme') as 'light' | 'dark' | null;
    const dark = saved ? saved === 'dark' : false;
    document.documentElement.classList.toggle('theme-dark', dark);
    queueMicrotask(() => {
      const chk = document.getElementById('loginThemeSwitch') as HTMLInputElement | null;
      if (chk) chk.checked = !dark; // checked = Light
    });
  }

  onThemeToggle(e: Event) {
    const isLight = (e.target as HTMLInputElement).checked;
    document.documentElement.classList.toggle('theme-dark', !isLight);
    localStorage.setItem('theme', isLight ? 'light' : 'dark');
  }

  onSubmit() {
    if (!this.email || !this.password) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const loginData: LoginRequest = {
      email: this.email,
      password: this.password
    };

    this.auth.login(loginData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.router.navigateByUrl('/dashboard');
        } else {
          this.errorMessage = response.message || 'Login failed';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Login failed. Please check your credentials.';
        console.error('Login error:', error);
      }
    });
  }
}

export class LoginComponent {
}
