import { Component } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent {
  applyForm = new FormGroup({
    userName: new FormControl(''),
    password: new FormControl(''),
  });

  constructor() {

  }

  submitLogin() { }
}
