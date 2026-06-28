import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Observable } from 'rxjs';
import { ResultadoPesquisaDto } from '../../dtos/pesquisa/pesquisa.dto';

@Injectable({ providedIn: 'root' })
export class PesquisaService {
  private readonly endpoint = 'pesquisa';

  constructor(private api: Api) {}

  pesquisar(termo: string, tipo: string = 'tudo'): Observable<ResultadoPesquisaDto> {
    return this.api.get<ResultadoPesquisaDto>(`${this.endpoint}?termo=${encodeURIComponent(termo)}&tipo=${tipo}`);
  }
}
