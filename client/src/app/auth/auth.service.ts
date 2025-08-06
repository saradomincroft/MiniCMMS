import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = environment.apiUrl;
  private role: string | null = null;

  constructor(private http: HttpClient, private router: Router) {}

  setUserRole(role: string) {
    this.role = role;
    localStorage.setItem('userRole', role);
  }

  getUserRole(): string | null {
    if (!this.role) {
        this.role = localStorage.getItem('userRole');
    }
    return this.role;
  }

  login(identifier: string, password: string): Observable<any> {
    const body = { identifier, password };
    return this.http.post(`${this.apiUrl}/auth/login`, body);
  }

  register(data: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/auth/register`, data);
  }

  logout() : void {
    this.role = null;
    localStorage.removeItem('userRole');
    localStorage.removeItem('authToken');

    this.router.navigate(['/login']);
  }
}
