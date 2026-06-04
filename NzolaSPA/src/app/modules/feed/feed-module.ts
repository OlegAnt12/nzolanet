import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FeedPrincipalComponent } from './pages/feed-principal.component/feed-principal.component';
import { PerfilComponent } from './pages/perfil.component/perfil.component';

const routes: Routes = [
  {
    path: '',
    component: FeedPrincipalComponent
  },
  {
    path: 'perfil',
    component: PerfilComponent
  },
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule, RouterModule.forChild(routes)
  ]
})
export class FeedModule { }
