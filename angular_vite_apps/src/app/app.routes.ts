import { Routes } from '@angular/router';
import { DetailsComponent } from './features/details/details.component';
import { LoginPageComponent } from './features/login-page/login-page.component';

export const routes: Routes = [
  {
    path: '',
    component: LoginPageComponent,
    title: 'Home Page',
  },
  {
    path: 'details/:id',
    component: DetailsComponent,
    title: 'Home Details',
  },
  {
    path: '**',
    redirectTo: '',
  },
];
