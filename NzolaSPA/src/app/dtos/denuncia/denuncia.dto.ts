export class CriarDenunciaDto {
  tipoEntidade: number = 0;
  idEntidade: number = 0;
  motivo: string = '';
  descricao: string = '';
  denuncianteId: number = 0;
}

export class DenunciaDto {
  id: number = 0;
  tipoEntidade: number = 0;
  idEntidade: number = 0;
  motivo: string = '';
  descricao: string = '';
  denuncianteId: number = 0;
  nomeDenunciante: string = '';
  dataDenuncia: string = '';
  estadoDenuncia: number = 0;
}
