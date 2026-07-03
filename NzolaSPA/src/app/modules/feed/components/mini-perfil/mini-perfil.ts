import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';

@Component({
  selector: 'app-mini-perfil',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Base64ImagePipe],
  templateUrl: './mini-perfil.html',
  styleUrl: './mini-perfil.css',
})
export class MiniPerfil {
  base64Image(base64String: string | undefined | null): string {
    if (!base64String) return './profile/Deafultdavy3k.jfif';
    if (base64String.startsWith('data:image')) return base64String;
    let mimeType = 'image/jpeg';
    if (base64String.startsWith('iVBOR')) mimeType = 'image/png';
    else if (base64String.startsWith('R0lGOD')) mimeType = 'image/gif';
    else if (base64String.startsWith('UklGR')) mimeType = 'image/webp';
    return `data:${mimeType};base64,${base64String}`;
  }

  @Input() utilizadorLogado: any;
  @Input() estatisticas: any;
  @Input() modoEdicao = false;

  @Output() abrirEdicao = new EventEmitter<void>();
  @Output() salvarPerfil = new EventEmitter<{ nomeCompleto: string; biografia: string }>();
  @Output() cancelarEdicao = new EventEmitter<void>();
  @Output() fotoSelecionada = new EventEmitter<File>();

  perfilForm = new FormGroup({
    nomeCompleto: new FormControl('', Validators.required),
    biografia: new FormControl(''),
  });

  ngOnChanges(): void {
    if (this.utilizadorLogado && this.modoEdicao) {
      this.perfilForm.patchValue({
        nomeCompleto: this.utilizadorLogado.nomeCompleto || '',
        biografia: this.utilizadorLogado.biografia || '',
      });
    }
  }

  onAbrirEdicao(): void {
    this.perfilForm.patchValue({
      nomeCompleto: this.utilizadorLogado?.nomeCompleto || '',
      biografia: this.utilizadorLogado?.biografia || '',
    });
    this.abrirEdicao.emit();
  }

  onSalvarPerfil(): void {
    if (this.perfilForm.invalid) return;
    this.salvarPerfil.emit({
      nomeCompleto: this.perfilForm.value.nomeCompleto!,
      biografia: this.perfilForm.value.biografia || '',
    });
  }

  onFotoSelecionada(event: any): void {
    if (event.target.files?.length) {
      this.fotoSelecionada.emit(event.target.files[0]);
    }
  }
}
