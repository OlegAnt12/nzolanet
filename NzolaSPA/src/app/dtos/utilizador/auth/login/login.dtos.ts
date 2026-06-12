export class LoginDtos {
  identificador: string='';
  palavraPasse: string='';
}

export class LoginResponseDto {
  token: string='';
  id: number=0;
  email: string='';
  nomeUtilizador: string='';
}