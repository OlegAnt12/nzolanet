import { Injectable } from '@angular/core';
import { Api } from '../api/api';
import { PedidoSeguirDto } from '../../dtos/seguidor/pedido-seguir.dto';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PedidoSeguirService {
  private readonly endpoint = 'pedidosseguir';

  constructor(private api: Api) {}

  solicitarSeguimento(seguidorId: number, seguidoId: number): Observable<PedidoSeguirDto> {
    return this.api.post<PedidoSeguirDto>(`${this.endpoint}/${seguidorId}/${seguidoId}`, {});
  }

  listarPendentes(utilizadorId: number): Observable<PedidoSeguirDto[]> {
    return this.api.get<PedidoSeguirDto[]>(`${this.endpoint}/pendentes/${utilizadorId}`);
  }

  aceitarPedido(pedidoId: number): Observable<PedidoSeguirDto> {
    return this.api.put<PedidoSeguirDto>(this.endpoint, `${pedidoId}/aceitar`, {});
  }

  rejeitarPedido(pedidoId: number): Observable<PedidoSeguirDto> {
    return this.api.put<PedidoSeguirDto>(this.endpoint, `${pedidoId}/rejeitar`, {});
  }
}
