import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../../services/auth/auth';

@Component({
  selector: 'app-redefinir-password',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './redefinir-password.component.html',
  styleUrl: './redefinir-password.component.css'
})
export class RedefinirPasswordComponent implements OnInit {
  token = '';
  novaPalavraPasse = '';
  confirmarPalavraPasse = '';
  redefinido = false;
  carregando = false;
  tokenInvalido = false;
  erro: string | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.token = this.route.snapshot.queryParams['token'] || '';
    if (!this.token) {
      this.tokenInvalido = true;
    }
  }

  onSubmit(): void {
    if (!this.token || this.novaPalavraPasse.length < 6 || this.novaPalavraPasse !== this.confirmarPalavraPasse) return;

    this.carregando = true;
    this.erro = null;

    this.authService.redefinirPassword(this.token, this.novaPalavraPasse).subscribe({
      next: () => {
        this.redefinido = true;
        this.carregando = false;
      },
      error: () => {
        this.erro = 'Token inválido ou expirado. Tenta novamente.';
        this.carregando = false;
      },
    });
  }

  irParaLogin(): void {
    this.router.navigate(['/home/login']);
  }
}
