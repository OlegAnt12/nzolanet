import { Component, OnInit } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft, faTrash, faCheck, faEye, faBan } from '@fortawesome/free-solid-svg-icons';
import { CommonModule } from '@angular/common';
import { Observable } from 'rxjs';
import { AdminService } from '../../../../services/admin/admin.service';
import { DenunciaDto } from '../../../../dtos/denuncia/denuncia.dto';

@Component({
  selector: 'app-denuncias.component',
  imports: [FontAwesomeModule, CommonModule],
  templateUrl: './denuncias.component.html',
  styleUrl: './denuncias.component.css',
})
export class DenunciasComponent implements OnInit {
  voltarIcon = faArrowLeft;
  removerIcon = faTrash;
  resolverIcon = faCheck;
  visualizarIcon = faEye;
  ignorarIcon = faBan;

  denuncias: DenunciaDto[] = [];
  carregando = true;
  erro: string | null = null;
  acaoEmCurso: number | null = null;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.carregarDenuncias();
  }

  private carregarDenuncias(): void {
    this.carregando = true;
    this.erro = null;
    this.adminService.listarDenuncias().subscribe({
      next: (dados) => {
        this.denuncias = dados;
        this.carregando = false;
      },
      error: () => {
        this.erro = 'Não foi possível carregar a lista de denúncias.';
        this.carregando = false;
      },
    });
  }

  removerConteudoDenunciado(tipo: number, idEntidade: number, denunciaId: number): void {
    const confirmacao = confirm('Tem a certeza que deseja remover este conteúdo? Esta ação é irreversível.');
    if (!confirmacao) return;

    this.acaoEmCurso = denunciaId;

    let obs: Observable<void>;

    switch (tipo) {
      case 1:
        obs = this.adminService.eliminarComentario(idEntidade);
        break;
      case 3:
        obs = this.adminService.eliminarPublicacao(idEntidade);
        break;
      default:
        alert('Tipo de conteúdo não suportado para remoção.');
        this.acaoEmCurso = null;
        return;
    }

    obs.subscribe({
      next: () => {
        this.adminService.atualizarEstadoDenuncia(denunciaId, 1).subscribe({
          next: () => {
            this.carregarDenuncias();
            this.acaoEmCurso = null;
          },
          error: () => {
            alert('Conteúdo removido, mas erro ao atualizar estado da denúncia.');
            this.carregarDenuncias();
            this.acaoEmCurso = null;
          },
        });
      },
      error: () => {
        alert('Erro ao remover o conteúdo denunciado.');
        this.acaoEmCurso = null;
      },
    });
  }

  marcarResolvida(id: number): void {
    this.acaoEmCurso = id;
    this.adminService.atualizarEstadoDenuncia(id, 1).subscribe({
      next: () => {
        this.carregarDenuncias();
        this.acaoEmCurso = null;
      },
      error: () => {
        alert('Erro ao atualizar estado da denúncia.');
        this.acaoEmCurso = null;
      },
    });
  }

  marcarIgnorada(id: number): void {
    this.acaoEmCurso = id;
    this.adminService.atualizarEstadoDenuncia(id, 2).subscribe({
      next: () => {
        this.carregarDenuncias();
        this.acaoEmCurso = null;
      },
      error: () => {
        alert('Erro ao atualizar estado da denúncia.');
        this.acaoEmCurso = null;
      },
    });
  }

  getTipoEntidadeLabel(tipo: number): string {
    switch (tipo) {
      case 1: return 'Comentário';
      case 2: return 'Perfil';
      case 3: return 'Publicação';
      default: return 'Desconhecido';
    }
  }
}
