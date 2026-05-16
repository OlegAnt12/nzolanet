import { Routes } from '@angular/router';
import { HOME_ROUTES } from './modules/home/home.routes';

export const routes: Routes = [
  /*{ path: '',   loadChildren: () => import('./modules/home/home.routes').then((m) => m.HOME_ROUTES) },
  { path: 'home', loadChildren: () => import('./modules/home/home.routes').then((m) => m.HOME_ROUTES) }, 
  */{
    path: 'feed',
    loadComponent: () =>
      import('./modules/feed/feed').then((m) => m.Feed),
  },
  {
    path: '**',
    loadComponent: () =>
      import('./modules/naosituado/naosituado').then((m) => m.Naosituado),
  },
];
