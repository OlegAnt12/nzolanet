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

  alternarBaze(publicacaoId: number, utilizadorId: number, bazeDto: any = {}): Observable<any> {
    // Monta o link exatamente como o [HttpPost("{publicacaoId:int}/{utilizadorId:int}")] do C# pede
    return this.api.post<any>(`${this.endpoint}/${publicacaoId}/${utilizadorId}`, bazeDto);
  }
}