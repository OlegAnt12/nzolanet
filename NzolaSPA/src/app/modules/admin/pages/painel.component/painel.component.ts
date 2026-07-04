import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft, faFileExport, faSearch, faUsers, faFileAlt, faFire, faFlag, faHourglassHalf, faUserCheck, faUserLock } from '@fortawesome/free-solid-svg-icons';
import { AdminService } from '../../../../services/admin/admin.service';
import { AdminDashboardDto } from '../../../../dtos/admin/admin-dashboard.dto';
import { UtilizadorDto } from '../../../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-painel.component',
  imports: [FontAwesomeModule, CommonModule, FormsModule],
  templateUrl: './painel.component.html',
  styleUrl: './painel.component.css',
})
export class PainelComponent implements OnInit {
  voltarIcon = faArrowLeft;
  exportarIcon = faFileExport;
  pesquisaIcon = faSearch;

  faUsers = faUsers;
  faFileAlt = faFileAlt;
  faFire = faFire;
  faFlag = faFlag;
  faHourglassHalf = faHourglassHalf;
  faUserCheck = faUserCheck;
  faUserLock = faUserLock;

  dashboard: AdminDashboardDto | null = null;
  carregando = true;
  erro: string | null = null;

  ultimosUtilizadores: UtilizadorDto[] = [];

  dataInicio: string = '';
  dataFim: string = '';
  periodo: string = '30';
  termoPesquisa: string = '';

  constructor(
    private adminService: AdminService,
    private router: Router,
  ) {}

  ngOnInit(): void {
    this.adminService.obterDashboard().subscribe({
      next: (dados) => {
        this.dashboard = dados;
        this.carregando = false;
      },
      error: () => {
        this.erro = 'Não foi possível carregar as estatísticas.';
        this.carregando = false;
      },
    });

    this.adminService.listarUtilizadores().subscribe({
      next: (dados) => {
        this.ultimosUtilizadores = dados.slice(0, 5);
      },
    });
  }

  voltar(): void {
    window.history.back();
  }

  exportarRelatorio(): void {
    const linhas: string[] = [
      'Relatório NzolaNet - ' + new Date().toLocaleDateString('pt-PT'),
      '---',
      'Total Utilizadores: ' + (this.dashboard?.totalUtilizadores ?? 0),
      'Total Publicações: ' + (this.dashboard?.totalPublicacoes ?? 0),
      'Total Bazes: ' + (this.dashboard?.totalBazes ?? 0),
      'Total Denúncias: ' + (this.dashboard?.totalDenuncias ?? 0),
      'Denúncias Pendentes: ' + (this.dashboard?.denunciasPendentes ?? 0),
      'Contas Ativas: ' + (this.dashboard?.utilizadoresAtivos ?? 0),
      'Perfis Privados: ' + (this.dashboard?.utilizadoresPrivados ?? 0),
      '',
      'Gerado em: ' + new Date().toISOString(),
    ];

    const blob = new Blob([linhas.join('\n')], { type: 'text/plain;charset=utf-8' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'relatorio-nzolanet-' + new Date().toISOString().slice(0, 10) + '.txt';
    a.click();
    URL.revokeObjectURL(url);
  }

  irParaListaCompleta(): void {
    this.router.navigate(['/admin/utilizadores']);
  }

  pesquisar(): void {
    if (this.termoPesquisa.trim()) {
      this.router.navigate(['/admin/utilizadores'], {
        queryParams: { termo: this.termoPesquisa.trim() },
      });
    }
  }

  onPeriodoChange(): void {
    const dias = parseInt(this.periodo, 10);
    const fim = new Date();
    const inicio = new Date();
    inicio.setDate(fim.getDate() - dias);
    this.dataInicio = inicio.toISOString().slice(0, 10);
    this.dataFim = fim.toISOString().slice(0, 10);
  }

  get nivelLabel(): string {
    return this.dashboard?.totalUtilizadores === 1 ? 'Administrador' : 'Administrador';
  }
}
