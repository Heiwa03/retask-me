import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../data/services/auth.service';

@Component({
  selector: 'app-root-gate',
  standalone: true,
  template: '<div class="gate"></div>',
  styles: [
    '.gate{min-height:100vh;background:var(--bg)}'
  ]
})
export class RootGate {
  private auth = inject(AuthService);
  private router = inject(Router);

  ngOnInit() {
    if (!this.auth.isAuthenticated()) {
      this.auth.login();
    }
    this.router.navigateByUrl('/dashboard');
  }
}


