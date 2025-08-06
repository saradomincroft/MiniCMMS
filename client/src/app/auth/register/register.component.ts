import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-auth',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html',
})
export class RegisterComponent {
  firstName = '';
  lastName = '';
  email = '';
  password = '';
  confirmPassword = '';
  role = 'technician';
  message = '';

  constructor(private http: HttpClient) {}

  register() {
    this.message = '';

    if (this.password !== this.confirmPassword) {
        this.message = 'Passwords don\'t match.';
        return;
    }

    if (!this.email || !this.firstName || !this.lastName || !this.password || !this.confirmPassword) {
        this.message = 'Please fill in all required fields.';
        return;
    }

    const registerDto = {
        firstName: this.firstName,
        lastName: this.lastName,
        email: this.email,
        password: this.password,
        role: this.role
    };

    this.http.post('http://localhost:5277/api/auth/register', registerDto).subscribe({
        next: (res) => {
            console.log('Registered: ', res);
            this.message = 'Registration successful';
        },
        error: (err) => {
            console.error('Registration failed:', err);
            this.message = 'Registration failed, please try again';
        }
    });
  }
}
