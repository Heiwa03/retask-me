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
