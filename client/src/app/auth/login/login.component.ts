import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router'; // ⬅️ import Router

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html',
})
export class LoginComponent {
  identifier = '';
  password = '';
  message = '';

  constructor(private http: HttpClient, private router: Router) {} // ⬅️ inject Router

  login() {
    const loginDto = { identifier: this.identifier, password: this.password };

    this.http.post<any>('http://localhost:5277/api/auth/login', loginDto).subscribe({
      next: (res) => {
        console.log('Logged in:', res);

        const role = res.role;

        // Save token if you want to
        localStorage.setItem('authToken', res.token);

        // Redirect based on role
        if (role === 'Manager') {
          this.router.navigate(['/manager-dashboard']);
        } else if (role === 'Technician') {
          this.router.navigate(['/technician-dashboard']);
        } else {
          this.message = 'Unknown role.';
        }
      },
      error: (err) => {
        console.error('Login failed:', err);
        this.message = 'Invalid username or password';
      }
    });
  }
}
