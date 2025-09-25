import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import { Observable, map, catchError, of } from 'rxjs';

export interface RegisterDetailsRequest {
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  gender: string;
}

export interface RegisterDetailsResponse {
  success: boolean;
  message?: string;
}

@Injectable({
  providedIn: 'root'
})
export class RegisterDetailsService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'https://retaskme-h0b8chg9fte5gme9.westeurope-01.azurewebsites.net/api/Auth';

  submitRegisterDetails(details: RegisterDetailsRequest): Observable<RegisterDetailsResponse> {
    return this.http.post<string>(`${this.baseUrl}/register-details`, details, {
      headers: { 'Content-Type': 'application/json' },
      responseType: 'text' as 'json'
    }).pipe(
      map((response: string) => ({
        success: response.includes('success') || response.includes('Success'),
        message: response
      })),
      catchError((error: any) => of({
        success: false,
        message: error.error || 'Failed to submit details'
      }))
    );
  }
}
