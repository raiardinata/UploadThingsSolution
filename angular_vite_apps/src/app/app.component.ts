import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';
import { LoginPageComponent } from './features/login-page/login-page.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LoginPageComponent, RouterOutlet, RouterLink],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'angular_vite_apps';
}
