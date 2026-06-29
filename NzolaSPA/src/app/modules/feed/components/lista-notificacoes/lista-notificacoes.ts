import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';

@Component({
  selector: 'app-lista-notificacoes',
  imports: [CommonModule, Base64ImagePipe],
  templateUrl: './lista-notificacoes.html',
  styleUrl: './lista-notificacoes.css',
})
export class ListaNotificacoes {
  @Input() notificacoes: any[] = [];

  getTipoLabel(tipo: number): string {
    const tipos = ['Baze', 'Comentário', 'Seguidor'];
    return tipos[tipo] || 'Desconhecido';
  }
}
