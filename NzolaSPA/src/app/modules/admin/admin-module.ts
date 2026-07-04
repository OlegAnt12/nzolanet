import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UtilizadoresComponent } from './pages/utilizadores.component/utilizadores.component';
import { PainelComponent } from './pages/painel.component/painel.component';
import { RouterModule, Routes } from '@angular/router';
import { PublicacoesComponent } from './pages/publicacoes.component/publicacoes.component';
import { DenunciasComponent } from './pages/denuncias.component/denuncias.component';
import { Layout } from './layout/layout';

const routes: Routes = [
  {
    path: '',
    component: Layout,
    children: [
      {
        path: 'painel',
        component: PainelComponent
      },
      {
        path: 'utilizadores',
        component: UtilizadoresComponent
      },
      {
        path: 'publicacoes',
        component: PublicacoesComponent
      },
      {
        path: 'denuncias',
        component: DenunciasComponent
      },
      { path: '', redirectTo: 'painel', pathMatch: 'full' }
    ]
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule, RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class AdminModule { }
