export interface PedidoSeguirDto {
  id: number;
  seguidorId: number;
  nomeSeguidor: string;
  nomeUtilizadorSeguidor: string;
  fotoSeguidor: string | null;
  seguidoId: number;
  nomeSeguido: string;
  estado: number;
  dataPedido: string;
}
