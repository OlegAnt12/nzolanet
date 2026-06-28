import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { NotificacaoDto, NovaNotificacaoDto } from '../../dtos/notificacao/notificacao.dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class NotificacaoService {
  private readonly endpoint = 'notificacao';

  constructor(private api: Api) {}

  listarPorUtilizador(utilizadorId: number): Observable<NotificacaoDto[]> {
    return this.api.get<NotificacaoDto[]>(`${this.endpoint}/utilizador/${utilizadorId}`);
  }
  criarNotificacao(utilizadorId: number, notificacao: NovaNotificacaoDto): Observable<NovaNotificacaoDto>
    {
      return this.api.post<NovaNotificacaoDto>(
        `${this.endpoint}/${utilizadorId}`,
        notificacao
      );
    }
}
