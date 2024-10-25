import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  // templateUrl: './app.component.html',
  template: `Hello`,
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'angular_vite_apps';
}
