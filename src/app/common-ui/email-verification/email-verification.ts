import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-email-verification',
  imports: [],
  templateUrl: './email-verification.html',
  styleUrls: ['./email-verification.scss']
})

export class EmailVerification implements OnInit{
  message = 'Verifying your email...';

  constructor(private http: HttpClient, private route: ActivatedRoute) {}

  ngOnInit() {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.message = 'Invalid verification link.';
      return;
    }

    this.http.get( `https://retaskme-h0b8chg9fte5gme9.westeurope-01.azurewebsites.net/api/v1/Email/verify-email?token=${token}`, { responseType: 'text' })
      .subscribe({
        next: () => this.message = 'Your email has been verified successfully!',
        error: () => this.message = 'Failed to verify email.'
      });
  }
}
