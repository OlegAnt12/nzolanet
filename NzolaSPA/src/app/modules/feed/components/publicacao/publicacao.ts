import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Base64ImagePipe } from '../../../../core/pipes/base64-image.pipe';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-publicacao',
  imports: [CommonModule, Base64ImagePipe, ReactiveFormsModule],
  templateUrl: './publicacao.html',
  styleUrl: './publicacao.css',
})
export class Publicacao {
  @Input() publicacao: any;
  @Input() utilizadorLogadoId: number = 0;
  @Input() seguindoAutores = new Set<number>();

  @Output() baze = new EventEmitter<number>();
  @Output() comentar = new EventEmitter<number>();
  @Output() enviarComentario = new EventEmitter<{ publicacaoId: number; texto: string }>();
  @Output() alternarSeguir = new EventEmitter<number>();
  @Output() denunciar = new EventEmitter<number>();
  @Output() editar = new EventEmitter<{ id: number; texto: string }>();
  @Output() cancelarEdicao = new EventEmitter<void>();
  @Output() salvarEdicao = new EventEmitter<number>();
  @Output() eliminar = new EventEmitter<number>();

  comentarioForm = new FormGroup({
    textoComentario: new FormControl<string>('', { nonNullable: true, validators: [Validators.required] }),
  });

  textoEdicaoControl = new FormControl('', Validators.required);
  modoEdicao = false;

  get autorSeguido(): boolean {
    return this.publicacao?.autor?.id ? this.seguindoAutores.has(this.publicacao.autor.id) : false;
  }

  get ehAutor(): boolean {
    return this.publicacao?.autor?.id === this.utilizadorLogadoId;
  }

  onBaze(): void {
    this.baze.emit(this.publicacao.id);
  }

  onComentar(): void {
    this.comentar.emit(this.publicacao.id);
  }

  onEnviarComentario(): void {
    if (this.comentarioForm.invalid) return;
    this.enviarComentario.emit({
      publicacaoId: this.publicacao.id,
      texto: this.comentarioForm.controls.textoComentario.value,
    });
    this.comentarioForm.reset();
  }

  onAlternarSeguir(): void {
    if (this.publicacao?.autor?.id) {
      this.alternarSeguir.emit(this.publicacao.autor.id);
    }
  }

  onDenunciar(): void {
    this.denunciar.emit(this.publicacao.id);
  }

  iniciarEdicao(): void {
    this.modoEdicao = true;
    this.textoEdicaoControl.setValue(this.publicacao.texto);
  }

  cancelarEdicaoAcao(): void {
    this.modoEdicao = false;
    this.textoEdicaoControl.reset();
    this.cancelarEdicao.emit();
  }

  salvarEdicaoAcao(): void {
    if (this.textoEdicaoControl.invalid) return;
    this.salvarEdicao.emit(this.publicacao.id);
  }

  onEliminar(): void {
    if (confirm('Tens a certeza de que queres eliminar esta publicação?')) {
      this.eliminar.emit(this.publicacao.id);
    }
  }
}
