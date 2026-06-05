export class PublicacaoDto {
  id: number = 0;
  utilizadorId: number = 0;
  nomeAutor: string = '';
  fotoAutor: string = '';
  texto: string = '';
  imagemUrl: string = '';
  videoUrl: string = '';
  dataPublicacao: string = '';
  numeroBazes: number = 0;
  numeroComentarios: number = 0;
  utilizadorJaDeuBaze: boolean = false;
  bazeId: number | null = null;
}


