import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable, map, catchError, of} from 'rxjs';

export interface ForgotPasswordRequest {
  email: string;
}

export interface ForgotPasswordResponse {
  success: boolean;
  message: string;
}

@Injectable({
  providedIn: 'root'
})
export class ForgotPasswordService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'https://retaskme-h0b8chg9fte5gme9.westeurope-01.azurewebsites.net/api/Auth';

  sendResetEmail(payload: ForgotPasswordRequest): Observable<ForgotPasswordResponse> {
    return this.http.post<string>(`${this.baseUrl}/forgot-password`, payload, {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text' as 'json'
    }).pipe(
      map((response: string) => ({
        success: response.includes('success') || response.includes('sent') || response.includes('email'),
        message: response
      })),
      catchError(error => of({
        success: false,
        message: error.error || 'Failed to send reset email'
      }))
    );
  }
  getTestForgotPassword() {
        return this.http.get(`${this.baseUrl}/forgot-password`);
    }

  // PASUL REAL: trimite emailul de resetare
  // sendResetEmail(payload: ForgotPasswordRequest): Observable<ForgotPasswordResponse> {
  //   return this.http.post<ForgotPasswordResponse>(`${this.baseUrlApi}/forgot-password`, payload);
  // }
}
