import { Routes } from '@angular/router';
import { PaginaHome } from './componentes/pagina-home/pagina-home';

export const HOME_ROUTES: Routes = [
  {
    path: '',
    component: PaginaHome,
    children: [
      {
        path: 'login',
        loadComponent: () => import('./paginas/login/login').then(m => m.Login)
      },
      {
        path: 'registo',
        loadComponent: () => import('./paginas/registo/registo').then(m => m.Registo)
      },
      // Rota padrão do painel (redireciona para o analítico)
      { path: '', redirectTo: 'login', pathMatch: 'full' }
    ]
  }
];