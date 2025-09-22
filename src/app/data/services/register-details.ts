import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RegisterDetailsService {
  http = inject(HttpClient);

  baseApiUrl = 'http://localhost:8080/api';//schimb dupa back-endul meu

  getTestRegisterDetails() {
    return this.http.get(`${this.baseApiUrl}/register-details`);
  }
}
