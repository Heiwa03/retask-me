import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormControl } from '@angular/forms';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss'
})
export class ForgotPassword {
  email = new FormControl('');

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
    if (this.email.value) {
      console.log('Trimit email la:', this.email.value);
      alert('Email trimis cu succes!');
    } else {
      alert('Te rog completează email-ul!');
    }
  }

  close() {
    console.log('Am apăsat close');
  }
}
