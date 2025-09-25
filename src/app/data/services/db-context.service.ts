import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DbCheckResponse {
  canConnect: boolean;
  userCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class DbContextService {
  private http = inject(HttpClient);
  private readonly baseUrl = 'https://retaskme-h0b8chg9fte5gme9.westeurope-01.azurewebsites.net/api/Auth';

  /**
   * Check database connection status
   * @returns Observable<DbCheckResponse> - Database connection status and user count
   */
  checkDatabaseConnection(): Observable<DbCheckResponse> {
    return this.http.get<DbCheckResponse>(`${this.baseUrl}/db-check`);
  }

  /**
   * Get database health status
   * @returns Promise<boolean> - True if database is healthy
   */
  async isDatabaseHealthy(): Promise<boolean> {
    try {
      const response = await this.checkDatabaseConnection().toPromise();
      return response?.canConnect === true;
    } catch (error) {
      console.error('Database health check failed:', error);
      return false;
    }
  }

  /**
   * Get current user count from database
   * @returns Promise<number> - Number of users in database
   */
  async getUserCount(): Promise<number> {
    try {
      const response = await this.checkDatabaseConnection().toPromise();
      return response?.userCount || 0;
    } catch (error) {
      console.error('Failed to get user count:', error);
      return 0;
    }
  }
}
