import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, RegisterRequest, UserRole } from '../../data/services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
  private router = inject(Router);
  private auth = inject(AuthService);
  mail = '';
  password = '';
  repeatPassword = '';
  role = UserRole.USER; // Default role as per Swagger
  isLoading = false;
  errorMessage = '';

  ngOnInit() {
    const saved = localStorage.getItem('theme') as 'light' | 'dark' | null;
    const dark = saved ? saved === 'dark' : false;
    document.documentElement.classList.toggle('theme-dark', dark);
    queueMicrotask(() => {
      const chk = document.getElementById('registerThemeSwitch') as HTMLInputElement | null;
      if (chk) chk.checked = !dark; // checked = Light
    });
  }

  onThemeToggle(e: Event) {
    const isLight = (e.target as HTMLInputElement).checked;
    document.documentElement.classList.toggle('theme-dark', !isLight);
    localStorage.setItem('theme', isLight ? 'light' : 'dark');
  }

  onSubmit() {
    if (!this.mail || !this.password || !this.repeatPassword) {
      this.errorMessage = 'Please fill in all fields';
      return;
    }

    if (this.password !== this.repeatPassword) {
      this.errorMessage = 'Passwords do not match';
      return;
    }

    if (this.password.length < 8) {
      this.errorMessage = 'Password must be at least 8 characters long';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';

    const registerData: RegisterRequest = {
      mail: this.mail,
      password: this.password,
      repeatPassword: this.repeatPassword,
      role: this.role
    };

    this.auth.register(registerData).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.router.navigateByUrl('/dashboard');
        } else {
          this.errorMessage = response.message || 'Registration failed';
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.errorMessage = 'Registration failed. Please try again.';
        console.error('Registration error:', error);
      }
    });
  }
}

export class RegisterComponent {
}
