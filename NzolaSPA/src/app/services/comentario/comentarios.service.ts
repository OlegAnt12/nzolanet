import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ComentarioDto } from '../../dtos/comentario/comentario-dto';
import { RequisicaoCriarComentarioDto } from '../../dtos/comentario/requisicao-criar-comentario-dto';
import { Api } from '../api/api';
import { RequisicaoEditarComentarioDto } from '../../dtos/comentario/requisicao-editar-comentario-dto';

@Injectable({
  providedIn: 'root',
})
export class ComentariosService {
  private readonly endpoint = 'comentarios';

  constructor(private api: Api)
    {
  
    }

    listarPorPublicacao(publicacaoId : number) : Observable <ComentarioDto[]>
    {
      return this.api.get<ComentarioDto[]>(`${this.endpoint}/publicacao/${publicacaoId}`);
    }

  buscarPorId(comentarioId: number): Observable <ComentarioDto>
  {
    return this.api.getById<ComentarioDto>(this.endpoint, comentarioId);
  }

  adicionarComentario(
    publicacaoId : number,
    utilizadorId : number,
    novoComentario: RequisicaoCriarComentarioDto): Observable<ComentarioDto>
  {
    return this.api.post<ComentarioDto>(
      `${this.endpoint}/${publicacaoId}/${utilizadorId}`,
      novoComentario
       );

  }

  editarComentario(
    comentarioId : number,
    comentarioEditado: RequisicaoEditarComentarioDto
  ) : Observable <ComentarioDto> {
    return this.api.put<ComentarioDto>(
      this.endpoint,
      comentarioId,
      comentarioEditado
    );
  }
  

  excluirComentario (comentarioId: number): Observable<void>
  {
    return this.api.delete<void>(this.endpoint, comentarioId);
  }
  
}
