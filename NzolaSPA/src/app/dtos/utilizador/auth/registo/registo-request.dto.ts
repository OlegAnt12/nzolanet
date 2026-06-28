export class RegistoRequestDto {
  nomeCompleto: string = '';
  nomeUtilizador: string ='';
  email: string = '';
  fotoPerfil: string | null = null;
  palavraPasse: string = '';
  genero: number = 0;
  dataNascimento: string = '';
  concordaComTermos: boolean = false;
}
