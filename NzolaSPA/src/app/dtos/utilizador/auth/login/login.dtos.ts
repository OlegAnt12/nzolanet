export class LoginDtos {
  identificador: string='';
  palavraPasse: string='';
}

export class LoginResponseDto {
  token: string='';
  refreshToken: string='';
  id: string='';
  email: string='';
  nomeUtilizador: string='';
  utilizador: any=null;
}