import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../data/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class Dashboard {
  private auth = inject(AuthService);
  private router = inject(Router);
  isProfileOpen = false;
  userAvatar = 'https://via.placeholder.com/40x40/6366f1/ffffff?text=AY'; // Default avatar
  userName = 'Amelia Yeldiz';
  userEmail = 'amelia@retaskme.com';

  ngOnInit() {
    const saved = localStorage.getItem('theme') as 'light' | 'dark' | null;
    const dark = saved ? saved === 'dark' : false;
    document.documentElement.classList.toggle('theme-dark', dark);
    queueMicrotask(() => {
      const chk = document.getElementById('dashThemeSwitch') as HTMLInputElement | null;
      if (chk) chk.checked = !dark; // checked = Light
    });
  }

  onThemeToggle(e: Event) {
    const isLight = (e.target as HTMLInputElement).checked;
    document.documentElement.classList.toggle('theme-dark', !isLight);
    localStorage.setItem('theme', isLight ? 'light' : 'dark');
  }

  logout() {
    this.auth.logout();
    this.router.navigateByUrl('/login');
  }

  toggleProfile() {
    this.isProfileOpen = !this.isProfileOpen;
  }

  closeProfile() {
    this.isProfileOpen = false;
  }
}


