import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';

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

  constructor(private http: HttpClient) {}

  login() {
    const loginDto = { identifier: this.identifier, password: this.password };
    this.http.post('http://localhost:5277/api/auth/login', loginDto).subscribe({
      next: (res) => console.log('Logged in:', res),
      error: (err) => console.error('Login failed:', err)
    });
  }
}
