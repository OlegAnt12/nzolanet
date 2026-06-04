import { Injectable } from '@angular/core';
import { BazeDto } from '../../dtos/baze/baze.dto';
import { Api } from '../api/api';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BazeService {
  private readonly endpoint='bazes';

  constructor(private api: Api) {}

  listarPorPublicacao(publicacaoId : number) : Observable <BazeDto[]>
  {
    return this.api.get<BazeDto[]>(
      `${this.endpoint}/publicacao/${publicacaoId}`
    );
  }

  darBaze(
    publicacaoId : number, 
    utilizadorId : number
  ) : Observable<BazeDto>{
    return this.api.post<BazeDto>(
      `${this.endpoint}/${publicacaoId}/${utilizadorId}`,
      {}
    );
  }

  removerBaze(bazeId: number): Observable<void>
  {
    return this.api.delete<void>(this.endpoint, bazeId);
  }

  verificarBaze(
    publicacaoId: number, 
    utilizadorId: number
  ): Observable<boolean>
  {
    return this.api.get<boolean>(
      `${this.endpoint}/verificar/${publicacaoId}/${utilizadorId}`
    );
  }
}