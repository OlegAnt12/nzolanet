export interface PerfilPesquisaDto {
  id: number;
  nomeCompleto: string;
  nomeUtilizador: string;
  fotoPerfil: string | null;
  privacidade: number;
  biografia: string | null;
}

export interface ResultadoPesquisaDto {
  publicacoes: any[];
  perfis: PerfilPesquisaDto[];
}
