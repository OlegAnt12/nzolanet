import { Injectable } from '@angular/core';
import { Api} from '../api/api';
import { Observable } from 'rxjs';
import { PublicacaoDto } from '../../dtos/publicacao/publicacao.dto';
import { RequisicaoCriarPublicacaoDto } from '../../dtos/publicacao/requisicao-criar-publicacao.dto';

@Injectable({
  providedIn: 'root',
})
export class PublicacaoService {
  
  private readonly endpoint = 'Publicacoes';

  constructor(private api: Api)
  {

  }

  listarRecentes(): Observable<PublicacaoDto[]>
  {
    return this.api.get<PublicacaoDto[]>(`${this.endpoint}/`);
  }

  publicar(novaPublicacao: RequisicaoCriarPublicacaoDto): Observable<PublicacaoDto>
  {
    const formData = new FormData();
    
    // Anexa o texto da publicação
    formData.append('Texto', novaPublicacao.texto);
    
    // Anexa cada ficheiro do array para dentro do lote de envio
    if (novaPublicacao.ficheiros && novaPublicacao.ficheiros.length > 0) {
      novaPublicacao.ficheiros.forEach((ficheiro) => {
        // O nome 'Ficheiros' deve bater exatamente com o nome do parâmetro no teu IFormFile/IFormFileCollection do C#
        formData.append('Ficheiros', ficheiro, ficheiro.name);
      });
    }
    return this.api.post<PublicacaoDto>(this.endpoint, formData);
  }

}
