import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '',   loadComponent: () => import('./modules/home/home').then((m) => m.Home) },
  { path: 'home',   loadComponent: () => import('./modules/home/home').then((m) => m.Home) },
  {
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
