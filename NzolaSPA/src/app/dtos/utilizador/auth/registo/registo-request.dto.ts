export class RegistoRequestDto {
  nomeCompleto: string = '';
  nomeUtilizador: string ='';
  email: string = '';
  fotoPerfil: string | null = null; // Enviamos como string Base64 para o C# converter em byte[]
  palavraPasse: string = '';
  genero: number = 0; // Mapeado como o valor numérico do Enum do C#
  dataNascimento: string = '';
}
