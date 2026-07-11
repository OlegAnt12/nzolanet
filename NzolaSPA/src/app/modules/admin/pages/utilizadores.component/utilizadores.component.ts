import { Component, OnInit, OnDestroy } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft, faPlus, faTimes } from '@fortawesome/free-solid-svg-icons';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../../../services/admin/admin.service';
import { UtilizadorDto } from '../../../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { CriarUtilizadorAdminRequestDto } from '../../../../dtos/admin/criar-utilizador-admin.dto';
import { Subject, takeUntil, timeout, catchError, of } from 'rxjs';

@Component({
  selector: 'app-utilizadores.component',
  imports: [FontAwesomeModule, CommonModule, FormsModule],
  templateUrl: './utilizadores.component.html',
  styleUrl: './utilizadores.component.css',
})
export class UtilizadoresComponent implements OnInit, OnDestroy {
  voltarIcon = faArrowLeft;
  adicionarIcon = faPlus;
  fecharIcon = faTimes;

  utilizadores: UtilizadorDto[] = [];
  carregando = true;
  erro: string | null = null;

  modalAberto = false;
  formSubmetido = false;
  criarCarregando = false;
  criarErro: string | null = null;

  novoUtilizador = new CriarUtilizadorAdminRequestDto();

  private destroy$ = new Subject<void>();

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    console.log('UtilizadoresComponent.ngOnInit chamado');
    this.carregarUtilizadores();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private carregarUtilizadores(): void {
    this.carregando = true;
    this.erro = null;
    this.adminService.listarUtilizadores().pipe(
      timeout(15000),
      catchError((err) => {
        console.error('Erro ao carregar utilizadores:', err);
        this.erro = 'Não foi possível carregar a lista de utilizadores.';
        this.carregando = false;
        return of([]);
      }),
      takeUntil(this.destroy$),
    ).subscribe({
      next: (dados) => {
        this.utilizadores = dados;
        this.carregando = false;
      },
    });
  }

  abrirModal(): void {
    this.novoUtilizador = new CriarUtilizadorAdminRequestDto();
    this.formSubmetido = false;
    this.criarErro = null;
    this.modalAberto = true;
  }

  fecharModal(): void {
    this.modalAberto = false;
  }

  onSubmit(): void {
    this.formSubmetido = true;

    if (!this.novoUtilizador.nomeCompleto || !this.novoUtilizador.nomeUtilizador ||
        !this.novoUtilizador.email || !this.novoUtilizador.palavraPasse) return;

    this.criarCarregando = true;
    this.criarErro = null;

    this.adminService.criarUtilizador(this.novoUtilizador).subscribe({
      next: () => {
        this.criarCarregando = false;
        this.fecharModal();
        this.carregarUtilizadores();
      },
      error: (err) => {
        this.criarCarregando = false;
        this.criarErro = err.error?.title || err.error || 'Erro ao criar utilizador.';
      },
    });
  }
}
