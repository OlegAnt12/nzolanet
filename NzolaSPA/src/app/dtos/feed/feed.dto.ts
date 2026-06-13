export class FeedDto {
}

export class AutorDto {
    id: number =0;
    nomeCompleto: string ='';
    fotoPerfil?: number[]=[];
    jaSegues?: boolean=false; // <-- Adiciona esta linha para controlo visual
  }