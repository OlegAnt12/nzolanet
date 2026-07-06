import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../../services/auth/auth';

@Component({
  selector: 'app-esqueci-password',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './esqueci-password.component.html',
  styleUrl: './esqueci-password.component.css'
})
export class EsqueciPasswordComponent {
  email = '';
  enviado = false;
  carregando = false;
  erro: string | null = null;

  constructor(private authService: AuthService) {}

  onSubmit(): void {
    if (!this.email) return;

    this.carregando = true;
    this.erro = null;

    this.authService.esqueciPassword(this.email).subscribe({
      next: () => {
        this.enviado = true;
        this.carregando = false;
      },
      error: () => {
        this.erro = 'Ocorreu um erro ao processar o pedido. Tenta novamente.';
        this.carregando = false;
      },
    });
  }
}
