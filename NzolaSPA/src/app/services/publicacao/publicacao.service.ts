import { Injectable } from '@angular/core';
import { Api} from '../api/api';
import { Observable } from 'rxjs';
import { PublicacaoDto } from '../../dtos/publicacao/publicacao.dto';
import { RequisicaoCriarPublicacaoDto } from '../../dtos/publicacao/requisicao-criar-publicacao.dto';

@Injectable({
  providedIn: 'root',
})
export class PublicacaoService {
  
  private readonly endpoint = 'publicacoes';

  constructor(private api: Api)
  {

  }

  obterRecentes(): Observable<PublicacaoDto[]>
  {
    return this.api.get<PublicacaoDto[]>(`${this.endpoint}/recentes`);
  }

  publicar(novaPublicacao: RequisicaoCriarPublicacaoDto): Observable<PublicacaoDto>
  {
    return this.api.post<PublicacaoDto>(this.endpoint, novaPublicacao);
  }

}
