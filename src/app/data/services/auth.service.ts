import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject, tap, map, catchError, of } from 'rxjs';

// UserRole enum as per Swagger documentation
export enum UserRole {
  USER = 0,
  ADMIN = 100,
  SUPER_ADMIN = 200
}

// LoginDto interface as per Swagger documentation
export interface LoginRequest {
  email: string; // minLength: 1, format: email
  password: string; // minLength: 1
}

// RegisterDTO interface as per Swagger documentation
export interface RegisterRequest {
  mail: string; // minLength: 1, format: email
  password: string; // minLength: 8
  repeatPassword: string; // minLength: 1
  role: UserRole; // enum: 0, 100, 200
}

export interface AuthResponse {
  success: boolean;
  message?: string;
  token?: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private readonly storageKey = 'retask_auth_token';
  private readonly baseUrl = 'https://retaskme-h0b8chg9fte5gme9.westeurope-01.azurewebsites.net/api/Auth';
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(this.isAuthenticated());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  isAuthenticated(): boolean {
    return !!localStorage.getItem(this.storageKey);
  }

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http.post<string>(`${this.baseUrl}/login`, credentials, {
      headers: { 
        'accept': '*/*',
        'Content-Type': 'application/json' 
      },
      responseType: 'text' as 'json',
      observe: 'response'
    }).pipe(
      map((response: any) => {
        console.log('Login API Response:', response);
        
        // Check if status is 200 (success)
        if (response.status === 200) {
          const mockToken = btoa(JSON.stringify({ 
            email: credentials.email, 
            timestamp: Date.now() 
          }));
          localStorage.setItem(this.storageKey, mockToken);
          this.isAuthenticatedSubject.next(true);
          
          return {
            success: true,
            message: 'Login successful',
            token: mockToken
          };
        } else {
          return {
            success: false,
            message: response.body || 'Login failed'
          };
        }
      }),
      catchError(error => {
        console.error('Login API Error:', error);
        let errorMessage = 'Login failed';
        
        if (error instanceof ProgressEvent) {
          errorMessage = 'Network error - please check your connection and CORS settings';
        } else if (error.status === 401) {
          errorMessage = 'Invalid email or password';
        } else if (error.status === 400) {
          errorMessage = error.error || 'Bad Request';
        } else if (error.error) {
          errorMessage = error.error;
        } else if (error.message) {
          errorMessage = error.message;
        }
        
        const authResponse: AuthResponse = {
          success: false,
          message: errorMessage
        };
        return of(authResponse);
      })
    );
  }

  register(userData: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<string>(`${this.baseUrl}/register`, userData, {
      headers: { 
        'accept': '*/*',
        'Content-Type': 'application/json' 
      },
      responseType: 'text' as 'json',
      observe: 'response'
    }).pipe(
      map((response: any) => {
        console.log('Register API Response:', response);
        
        // Check if status is 200 (success)
        if (response.status === 200) {
          const mockToken = btoa(JSON.stringify({ 
            email: userData.mail, 
            timestamp: Date.now() 
          }));
          localStorage.setItem(this.storageKey, mockToken);
          this.isAuthenticatedSubject.next(true);
          
          return {
            success: true,
            message: 'Registration successful',
            token: mockToken
          };
        } else {
          return {
            success: false,
            message: response.body || 'Registration failed'
          };
        }
      }),
      catchError(error => {
        console.error('Register API Error:', error);
        let errorMessage = 'Registration failed';
        
        if (error instanceof ProgressEvent) {
          errorMessage = 'Network error - please check your connection and CORS settings';
        } else if (error.status === 400) {
          // Handle 400 Bad Request with specific error message
          errorMessage = error.error || 'Bad Request - Username may already exist';
        } else if (error.error) {
          errorMessage = error.error;
        } else if (error.message) {
          errorMessage = error.message;
        }
        
        const authResponse: AuthResponse = {
          success: false,
          message: errorMessage
        };
        return of(authResponse);
      })
    );
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.isAuthenticatedSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.storageKey);
  }

  // Test method to check if backend is reachable
  testConnection(): Observable<any> {
    return this.http.get('https://retaskme-h0b8chg9fte5gme9.westeurope-01.azurewebsites.net/api/Auth/db-check')
      .pipe(
        catchError(error => {
          console.error('Backend Connection Test Failed:', error);
          throw error;
        })
      );
  }
}


