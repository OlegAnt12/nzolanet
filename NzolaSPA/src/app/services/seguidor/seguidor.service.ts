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
      listarSeguidos(utilizadorId: number): Observable<number[]> {
        return this.api.get<number[]>(`${this.endpoint}/seguindo/${utilizadorId}`);
      }
}
