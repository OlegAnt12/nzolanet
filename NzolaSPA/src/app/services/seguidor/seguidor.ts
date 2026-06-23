import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Api } from '../api/api';
import { SeguidorDto } from '../../dtos/seguidor/seguidor.dto';

@Injectable({
  providedIn: 'root',
})
export class Seguidor {
  private readonly endpoint='seguidores';
  
    constructor(private api: Api) {}
  
    listarPorUtilizador(utilizadorId : number) : Observable <SeguidorDto[]>
    {
      return this.api.get<SeguidorDto[]>(
        `${this.endpoint}/utilizador/${utilizadorId}`
      );
    }
  
    alternarSeguir(seguidorId: number, seguidoId: number, seguidorDto: any = {}): Observable<any> {
      // Monta o link exatamente como o [HttpPost("{publicacaoId:int}/{utilizadorId:int}")] do C# pede
      return this.api.post<any>(`${this.endpoint}/${seguidorId}/${seguidoId}`, seguidorDto);
    }
}
