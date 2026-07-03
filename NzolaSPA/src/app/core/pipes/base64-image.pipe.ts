import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'base64Image',
  standalone: true
})
export class Base64ImagePipe implements PipeTransform {
  transform(base64String: string | undefined | null): string {
    if (!base64String || base64String.length === 0) {
      return './profile/Deafultdavy3k.jfif';
    }

    if (base64String.startsWith('data:image')) {
      return base64String;
    }

    let mimeType = 'image/jpeg';

    if (base64String.startsWith('iVBOR')) {
      mimeType = 'image/png';
    }
    else if (base64String.startsWith('R0lGOD')) {
      mimeType = 'image/gif';
    }
    else if (base64String.startsWith('UklGR')) {
      mimeType = 'image/webp';
    }

    return `data:${mimeType};base64,${base64String}`;
  }
}