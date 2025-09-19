import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

export interface ForgotPasswordRequest { email: string; }
export interface ForgotPasswordResponse { ok: boolean; message: string; }

@Injectable({
  providedIn: 'root'
})
export class ForgotPasswordService {
  http = inject(HttpClient);

  baseApiUrl = 'http://localhost:8080/api';//schimb dupa back-endul meu

  getTestForgotPassword() {
  return this.http.get(`${this.baseApiUrl}/forgot-password`);
  }

  // PASUL REAL: trimite emailul de resetare
  // sendResetEmail(payload: ForgotPasswordRequest): Observable<ForgotPasswordResponse> {
  //   return this.http.post<ForgotPasswordResponse>(`${this.baseUrlApi}/forgot-password`, payload);
  // }
}
