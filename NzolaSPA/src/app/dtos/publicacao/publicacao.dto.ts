import { UtilizadorSimplificadoDto } from "../utilizador/utilizadorfeed/utilizador.dto";

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
  ficheiros: PublicacaoFicheiroDto[]=[]; // <-- ADICIONA ESTA LINHA EXATAMENTE AQUI
  comentarios: any[]=[];
  autor?: UtilizadorSimplificadoDto;
}


export interface PublicacaoFicheiroDto {
  id: number;
  caminhoFicheiro: string;
  tipoMime: string;
  tamanhoBytes: number;
  dataUpload: string;
  urlCompleta?: string; // Propriedade auxiliar que crias no Angular
}


/*
export interface PublicacaoDto {
  id: number;
  texto: string;
  quantidadeBazes: number;
  quantidadeComentarios: number;
  dataPublicacao: string;
  autor: AutorDto;
  ficheiros: PublicacaoFicheiroDto[]; // <-- ADICIONA ESTA LINHA EXATAMENTE AQUI
  comentarios: any[];
}*/

