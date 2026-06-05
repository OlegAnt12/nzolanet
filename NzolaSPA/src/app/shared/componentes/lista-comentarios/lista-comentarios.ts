import { Component, Input, OnInit } from '@angular/core';
import { ComentarioDto } from '../../../dtos/comentario/comentario-dto';
import { ComentariosService } from '../../../services/comentario/comentarios.service';
import { CommonModule } from '@angular/common';
import { CartaoComentario } from '../cartao-comentario/cartao-comentario';

@Component({
  selector: 'app-lista-comentarios',
  imports: [CommonModule, CartaoComentario],
  templateUrl: './lista-comentarios.html',
  styleUrl: './lista-comentarios.css',
})
export class ListaComentarios implements OnInit {
  
  @Input() publicacaoId!: number;

  comentarios: ComentarioDto[] = [];

  constructor(private comentariosService: ComentariosService) {}

  ngOnInit(): void {
  this.carregarComentarios();
}

carregarComentarios(): void {
  this.comentariosService.listarPorPublicacao(this.publicacaoId).subscribe({
    next: (resposta) => {
      this.comentarios = resposta;
    },
    error: (erro) => {
      console.error('Erro ao carregar comentários', erro);
    },

    
  });

  
}

aoEditarComentario(comentario: ComentarioDto): void {
  console.log('Editar comentário:', comentario);
}

aoExcluirComentario(comentarioId: number): void {
  console.log('Excluir comentário:', comentarioId);
}

}
