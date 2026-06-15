import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Observable } from 'rxjs';
import { EstatisticasUtilizadorDto } from '../../dtos/utilizador/utilizadorfeed/utilizador.dto';

@Injectable({
  providedIn: 'root',
})
export class UtilizadorService {
  private readonly endpoint='utilizadores';
  constructor(private api: Api) {}

  obterEstatisticas(utilizadorId: number): Observable<EstatisticasUtilizadorDto> {
    return this.api.get<EstatisticasUtilizadorDto>(`utilizadores/${utilizadorId}/estatisticas`);
  }

  atualizarPerfil(utilizadorId: number, nomeCompleto: string, biografia: string, fotoFile?: File): Observable<any> {
    const formData = new FormData();
    formData.append('NomeCompleto', nomeCompleto);
    formData.append('Biografia', biografia);
    
    if (fotoFile) {
      formData.append('NovaFoto', fotoFile, fotoFile.name);
    }

    // Envia como multipart/form-data para a API do C#
    return this.api.put<any>(`${this.endpoint}/perfil`,utilizadorId, formData);
  }
}
