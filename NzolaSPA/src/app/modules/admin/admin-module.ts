import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UtilizadoresComponent } from './pages/utilizadores.component/utilizadores.component';
import { PainelComponent } from './pages/painel.component/painel.component';
import { RouterModule, Routes } from '@angular/router';
import { PublicacoesComponent } from './pages/publicacoes.component/publicacoes.component';
import { Layout } from './layout/layout';

const routes: Routes = [
  {
    path: '',
    component: Layout,
    // Rota pai '/admin' que serve de casca/layout para o painel
    children: [
      {
        path: 'painel',
        // Rota para '/admin/painel'
        component: PainelComponent
      },
      {
        path: 'utilizadores',
        // Rota para '/admin/utilizadores'
        component: UtilizadoresComponent
      },
      {
        path: 'publicacoes',
        // Rota para '/admin/publicacoes'
        component: PublicacoesComponent
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
