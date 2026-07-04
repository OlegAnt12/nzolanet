import { Component, OnInit } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { CommonModule } from '@angular/common';
import { AdminService } from '../../../../services/admin/admin.service';

interface AdminAutorDto {
  id: number;
  nomeCompleto: string;
  nomeUtilizador: string;
  fotoPerfil: string | null;
}

interface AdminPublicacaoDto {
  id: number;
  quantidadeBazes: number;
  quantidadeComentarios: number;
  dataPublicacao: string;
  autor: AdminAutorDto | null;
  texto: string;
}

@Component({
  selector: 'app-publicacoes.component',
  imports: [FontAwesomeModule, CommonModule],
  templateUrl: './publicacoes.component.html',
  styleUrl: './publicacoes.component.css',
})
export class PublicacoesComponent implements OnInit {
  voltarIcon = faArrowLeft;

  publicacoes: AdminPublicacaoDto[] = [];
  carregando = true;
  erro: string | null = null;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
    this.adminService.listarPublicacoes().subscribe({
      next: (dados: any) => {
        this.publicacoes = dados;
        this.carregando = false;
      },
      error: () => {
        this.erro = 'Não foi possível carregar a lista de publicações.';
        this.carregando = false;
      },
    });
  }
}
