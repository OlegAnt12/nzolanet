import { Injectable, NgZone } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Observable, Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SignalRService {
  private hubConnection: signalR.HubConnection | null = null;
  private notificacaoSubject = new Subject<any>();
  private bazeSubject = new Subject<any>();
  private conectadoSubject = new Subject<boolean>();

  notificacoes$: Observable<any> = this.notificacaoSubject.asObservable();
  baze$: Observable<any> = this.bazeSubject.asObservable();
  conectado$: Observable<boolean> = this.conectadoSubject.asObservable();

  constructor(private ngZone: NgZone) {}

  iniciarConexao(token: string): void {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) return;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5043/hubs/notifications', {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000, 30000])
      .build();

    this.hubConnection.on('ReceberNotificacao', (data: any) => {
      this.ngZone.run(() => this.notificacaoSubject.next(data));
    });

    this.hubConnection.on('AtualizarBaze', (data: any) => {
      this.ngZone.run(() => this.bazeSubject.next(data));
    });

    this.hubConnection.onreconnecting(() => {
      this.conectadoSubject.next(false);
    });

    this.hubConnection.onreconnected(() => {
      this.conectadoSubject.next(true);
    });

    this.hubConnection.onclose(() => {
      this.conectadoSubject.next(false);
    });

    this.hubConnection.start()
      .then(() => this.conectadoSubject.next(true))
      .catch(err => console.error('Erro ao conectar SignalR:', err));
  }

  pararConexao(): void {
    this.hubConnection?.stop();
    this.hubConnection = null;
  }
}
