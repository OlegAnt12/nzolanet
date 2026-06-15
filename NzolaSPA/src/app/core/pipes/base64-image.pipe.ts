import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'base64Image',
  standalone: true
})
export class Base64ImagePipe implements PipeTransform {
  transform(base64String: string | undefined | null): string {
    console.log('Pipe base64Image recebeu:', base64String ? base64String.substring(0, 50) + '...' : 'null/undefined');
    
    if (!base64String || base64String.length === 0) {
      return './profile/pexels-carlosfotografias-5669788.jpg'; // Avatar padrão
    }
    
    // Verifica se já tem o prefixo data:image
    if (base64String.startsWith('data:image')) {
      return base64String;
    }
    
    // Detecta o tipo da imagem pelos primeiros caracteres do Base64
    let mimeType = 'image/jpeg';
    
    // PNG começa com 'iVBOR' em Base64
    if (base64String.startsWith('iVBOR')) {
      mimeType = 'image/png';
    }
    // GIF começa com 'R0lGOD'
    else if (base64String.startsWith('R0lGOD')) {
      mimeType = 'image/gif';
    }
    // WebP começa com 'UklGR'
    else if (base64String.startsWith('UklGR')) {
      mimeType = 'image/webp';
    }
    
    // Adiciona o prefixo necessário
    return `data:${mimeType};base64,${base64String}`;
  }
}