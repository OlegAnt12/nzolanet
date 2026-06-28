export class UtilizadorDto {
  id: number = 0;
  nomeCompleto: string = '';
  nomeUtilizador: string = '';
  email: string = '';
  biografia: string = '';
  privacidade: number = 0;
  estadoConta: number = 0;
  fotoPerfil: string | null = null;
  genero: number = 0;
  dataNascimento: string = '';
  seguidores: number = 0;
  seguindo: number = 0;
  publicacoes: number = 0;
  jaSegues: boolean = false;
  concordaComTermos: boolean = false;
}

export class EstatisticasUtilizadorDto {
  seguidores: number = 0;
  seguindo: number = 0;
  publicacoes: number = 0;
}

export class UtilizadorSimplificadoDto {
  id: number = 0;
  nomeCompleto: string = '';
  nomeUtilizador: string = '';
  fotoPerfil: string | null = null;
}
