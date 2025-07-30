import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthComponent } from './auth/auth.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, AuthComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('client');
}
