import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class Register {
  private router = inject(Router);
  fullName = '';
  email = '';
  password = '';
  confirmPassword = '';

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
    const [firstName, ...rest] = this.fullName.trim().split(' ');
    const lastName = rest.join(' ');
    this.router.navigateByUrl('/register-details', {
      state: {
        firstName: firstName || '',
        lastName: lastName || '',
        email: this.email
      }
    });
  }
}

export class RegisterComponent {
}
