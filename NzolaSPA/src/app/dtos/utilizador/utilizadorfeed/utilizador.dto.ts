export class UtilizadorDto {
}
export class EstatisticasUtilizadorDto {
  seguidores: number=0;
  seguindo: number=0;
  publicacoes: number=0;
}

export class UtilizadorSimplificadoDto {
  id: number=0;
  nome: string='';
  fotoPerfil: string='';
  jaSegues: boolean=false; // ✅ Aqui sim! Estado de seguir pertence ao autor
  seguidores?: number=0;
}