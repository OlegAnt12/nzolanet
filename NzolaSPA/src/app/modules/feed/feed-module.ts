import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes, ResolveFn, ActivatedRouteSnapshot } from '@angular/router';
import { FeedPrincipalComponent } from './pages/feed-principal.component/feed-principal.component';
import { PerfilComponent } from './pages/perfil.component/perfil.component';
import { PesquisaComponent } from './pages/pesquisa/pesquisa.component';
import { inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID } from '@angular/core';
import { of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { UtilizadorService } from '../../services/utilizador/utilizador.service';
import { UtilizadorDto } from '../../dtos/utilizador/utilizadorfeed/utilizador.dto';

export const perfilResolver: ResolveFn<UtilizadorDto | null> = (route: ActivatedRouteSnapshot) => {
  const utilizadorService = inject(UtilizadorService);
  const platformId = inject(PLATFORM_ID);
  const idParam = route.paramMap.get('id');
  let id = idParam ? Number(idParam) : 0;

  if ((!id || Number.isNaN(id)) && isPlatformBrowser(platformId)) {
    const storedId = localStorage.getItem('utilizadorId');
    id = storedId ? Number(storedId) : 0;
  }

  if (!id || Number.isNaN(id)) {
    return of(null);
  }

  let utilizadorLogadoId: number | undefined;
  if (isPlatformBrowser(platformId)) {
    const storedId = localStorage.getItem('utilizadorId');
    utilizadorLogadoId = storedId ? Number(storedId) : undefined;
  }

  return utilizadorService.obterPorId(id, utilizadorLogadoId).pipe(
    catchError(() => of(null))
  );
};

const routes: Routes = [
  {
    path: '',
    component: FeedPrincipalComponent
  },
  {
    path: 'perfil',
    component: PerfilComponent,
    resolve: {
      perfil: perfilResolver,
    }
  },
  {
    path: 'perfil/:id',
    component: PerfilComponent,
    resolve: {
      perfil: perfilResolver,
    }
  },
  {
    path: 'pesquisa',
    component: PesquisaComponent
  },
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule, RouterModule.forChild(routes)
  ]
})
export class FeedModule { }
