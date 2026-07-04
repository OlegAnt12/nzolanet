import { Component, OnInit } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faArrowLeft } from '@fortawesome/free-solid-svg-icons';
import { CommonModule } from '@angular/common';
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

  denuncias: DenunciaDto[] = [];
  carregando = true;
  erro: string | null = null;

  constructor(private adminService: AdminService) {}

  ngOnInit(): void {
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
}
