import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/login.component/login.component';
import { RegistoComponent } from './pages/registo.component/registo.component';
import { EsqueciPasswordComponent } from './pages/esqueci-password.component/esqueci-password.component';
import { RedefinirPasswordComponent } from './pages/redefinir-password.component/redefinir-password.component';

const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'registo',
    component: RegistoComponent
  },
  {
    path: 'esqueci-password',
    component: EsqueciPasswordComponent
  },
  {
    path: 'redefinir-password',
    component: RedefinirPasswordComponent
  }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule, RouterModule.forChild(routes), LoginComponent, RegistoComponent, EsqueciPasswordComponent, RedefinirPasswordComponent
  ],
  exports : [RouterModule]
})
export class HomeModule { }
