import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-mini-perfil',
  imports: [CommonModule, Base64ImagePipe, ReactiveFormsModule],
  templateUrl: './mini-perfil.html',
  styleUrl: './mini-perfil.css',
})
export class MiniPerfil {
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
