import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { Observable } from 'rxjs';
import { SeguidorDto } from '../../dtos/seguidor/seguidor.dto';

@Injectable({
  providedIn: 'root',
})
export class SeguidorService {
  private readonly endpoint='seguidores';
    
      constructor(private api: Api) {}
    
      listarPorUtilizador(utilizadorId : number) : Observable <SeguidorDto[]>
      {
        return this.api.get<SeguidorDto[]>(
          `${this.endpoint}/utilizador/${utilizadorId}`
        );
      }
    
      alternarSeguir(seguidorId: number, seguidoId: number): Observable<any> {
        // Monta o link exatamente como o [HttpPost("{publicacaoId:int}/{utilizadorId:int}")] do C# pede
        return this.api.post<any>(`${this.endpoint}/${seguidorId}/${seguidoId}`, {});
      }

      // NOVO MÉTODO: Buscar todos que o usuário segue
      listarSeguidos(utilizadorId: number): Observable<any[]> {
        return this.api.get<any[]>(`${this.endpoint}/seguindo/${utilizadorId}`);
      }

      // Verifica se um utilizador segue outro DO:
  verificarSegue(seguidorId: number, seguidoId: number): Observable<boolean> {
    return this.api.get<boolean>(`${this.endpoint}/verificar/${seguidorId}/${seguidoId}`);
  }

  // Obtém todos os seguidores de um utilizador
  obterSeguidores(utilizadorId: number): Observable<any[]> {
    return this.api.get<any[]>(`${this.endpoint}/utilizador/${utilizadorId}`);
  }

  // Obtém todos os utilizadores que um utilizador segue
  obterSeguindo(utilizadorId: number): Observable<any[]> {
    return this.api.get<any[]>(`${this.endpoint}/seguindo/${utilizadorId}`);
  }
}
