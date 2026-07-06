import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Observable } from 'rxjs';
import { AdminDashboardDto } from '../../dtos/admin/admin-dashboard.dto';
import { UtilizadorDto } from '../../dtos/utilizador/utilizadorfeed/utilizador.dto';
import { DenunciaDto } from '../../dtos/denuncia/denuncia.dto';
import { CriarUtilizadorAdminRequestDto } from '../../dtos/admin/criar-utilizador-admin.dto';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private endpoint = 'admin';

  constructor(private api: Api) {}

  obterDashboard(): Observable<AdminDashboardDto> {
    return this.api.get<AdminDashboardDto>(`${this.endpoint}/dashboard`);
  }

  listarUtilizadores(): Observable<UtilizadorDto[]> {
    return this.api.get<UtilizadorDto[]>(`${this.endpoint}/utilizadores`);
  }

  listarPublicacoes(): Observable<any[]> {
    return this.api.get<any[]>(`${this.endpoint}/publicacoes`);
  }

  listarDenuncias(): Observable<DenunciaDto[]> {
    return this.api.get<DenunciaDto[]>(`${this.endpoint}/denuncias`);
  }

  atualizarEstadoDenuncia(id: number, estadoDenuncia: number): Observable<DenunciaDto> {
    return this.api.put<DenunciaDto>(`${this.endpoint}/denuncias`, `${id}/estado`, { estadoDenuncia });
  }

  eliminarComentario(id: number): Observable<void> {
    return this.api.delete<void>('comentarios', id);
  }

  eliminarPublicacao(id: number): Observable<void> {
    return this.api.delete<void>('publicacoes', id);
  }

  criarUtilizador(dto: CriarUtilizadorAdminRequestDto): Observable<UtilizadorDto> {
    return this.api.post<UtilizadorDto>(`${this.endpoint}/utilizadores`, dto);
  }
}
