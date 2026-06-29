import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Observable } from 'rxjs';
import { EstatisticasUtilizadorDto, UtilizadorDto } from '../../dtos/utilizador/utilizadorfeed/utilizador.dto';

@Injectable({
  providedIn: 'root',
})
export class UtilizadorService {
  private readonly endpoint='utilizadores';
  constructor(private api: Api) {}

  obterPorId(id: number, utilizadorLogadoId?: number): Observable<UtilizadorDto> {
    const query = utilizadorLogadoId ? `?utilizadorLogadoId=${utilizadorLogadoId}` : '';
    return this.api.get<UtilizadorDto>(`${this.endpoint}/${id}${query}`);
  }

  obterEstatisticas(utilizadorId: number): Observable<EstatisticasUtilizadorDto> {
    return this.api.getById<EstatisticasUtilizadorDto>(`${this.endpoint}/estatisticas`,utilizadorId);
  }

  atualizarPerfil(utilizadorId: number, nomeCompleto: string, biografia: string, fotoFile?: File): Observable<any> {
    const formData = new FormData();
    formData.append('NomeCompleto', nomeCompleto);
    formData.append('Biografia', biografia);
    
    if (fotoFile) {
      formData.append('NovaFoto', fotoFile, fotoFile.name);
    }

    return this.api.put<any>(`${this.endpoint}/perfil`,utilizadorId, formData);
  }
}
