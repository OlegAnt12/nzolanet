import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Observable } from 'rxjs';
import { CriarDenunciaDto, DenunciaDto } from '../../dtos/denuncia/denuncia.dto';

@Injectable({
  providedIn: 'root',
})
export class DenunciaService {
  private readonly endpoint = 'denuncias';

  constructor(private api: Api) {}

  criarDenuncia(dto: CriarDenunciaDto): Observable<any> {
    return this.api.post<any>(`${this.endpoint}`, dto);
  }

  listarDenuncias(): Observable<DenunciaDto[]> {
    return this.api.get<DenunciaDto[]>(`${this.endpoint}`);
  }

  listarPorEntidade(tipoEntidade: number, idEntidade: number): Observable<DenunciaDto[]> {
    return this.api.get<DenunciaDto[]>(`${this.endpoint}/entidade/${tipoEntidade}/${idEntidade}`);
  }
}
