export class ComentarioDto {

    id!: number;
    publicacaoId!: number;
    utilizadorId!: number;
     conteudoComentario!: string;
     dataComentario!: Date;
     dataActualizacao!: Date;
}

export class CriarComentarioDto {
    publicacaoId: number=0;
    utilizadorId: number=0;
    conteudoComentario: string='';
  }
  
  // DTO para receber um comentário do Servidor
  export class ComentarioRequestDto {
    id: number=0;
    publicacaoId: number=0;
    utilizadorId: number=0;
    conteudoComentario: string='';
    autorNome?: string='';
    dataCriacao?: string='';
  }
