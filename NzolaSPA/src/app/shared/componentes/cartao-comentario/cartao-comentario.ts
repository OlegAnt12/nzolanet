import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ComentarioDto } from '../../../dtos/comentario/comentario-dto';

@Component({
  selector: 'app-cartao-comentario',
  imports: [],
  templateUrl: './cartao-comentario.html',
  styleUrl: './cartao-comentario.css',
})
export class CartaoComentario {

  @Input() comentario! : ComentarioDto;
  @Output() editar = new EventEmitter<ComentarioDto>();
  @Output() excluir = new EventEmitter<number>();

  aoEditar(): void {
  this.editar.emit(this.comentario);
  }

aoExcluir(): void {
  this.excluir.emit(this.comentario.id);
 }

}
