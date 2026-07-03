import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';

@Component({
  selector: 'app-lista-notificacoes',
  standalone: true,
  imports: [CommonModule, Base64ImagePipe],
  templateUrl: './lista-notificacoes.html',
  styleUrl: './lista-notificacoes.css',
})
export class ListaNotificacoes {
  base64Image(base64String: string | undefined | null): string {
    if (!base64String) return './profile/Deafultdavy3k.jfif';
    if (base64String.startsWith('data:image')) return base64String;
    let mimeType = 'image/jpeg';
    if (base64String.startsWith('iVBOR')) mimeType = 'image/png';
    else if (base64String.startsWith('R0lGOD')) mimeType = 'image/gif';
    else if (base64String.startsWith('UklGR')) mimeType = 'image/webp';
    return `data:${mimeType};base64,${base64String}`;
  }

  @Input() notificacoes: any[] = [];

  getTipoLabel(tipo: number): string {
    const tipos = ['Baze', 'Comentário', 'Seguidor'];
    return tipos[tipo] || 'Desconhecido';
  }
}
