import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full'
  },
  { path: 'home',
    loadChildren: () =>
      import('./modules/home/home-module').then((m) => m.HomeModule)
  },
  {
    path: 'feed',
    loadChildren: () =>
      import('./modules/feed/feed-module').then((m) => m.FeedModule)
  },
  {
    path: 'admin',
    loadChildren: () =>
      import('./modules/admin/admin-module').then((m) => m.AdminModule)
  },
  {
    path: 'auth',
    loadChildren:() =>
      import('./modules/auth/auth.routes').then((m) => m.AUTH_ROUTES)
  },
  
  {
    path: '**',
    loadComponent: () =>
      import('./modules/naosituado/naosituado').then((m) => m.Naosituado)
  },
];
