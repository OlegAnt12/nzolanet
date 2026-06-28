import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FeedPrincipalComponent } from './pages/feed-principal.component/feed-principal.component';
import { PerfilComponent } from './pages/perfil.component/perfil.component';
import { PesquisaComponent } from './pages/pesquisa/pesquisa.component';

const routes: Routes = [
  {
    path: '',
    component: FeedPrincipalComponent
  },
  {
    path: 'perfil',
    component: PerfilComponent
  },
  {
    path: 'perfil/:id',
    component: PerfilComponent
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
