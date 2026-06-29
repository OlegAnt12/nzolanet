import { UtilizadorSimplificadoDto } from "../utilizador/utilizadorfeed/utilizador.dto";

export class NotificacaoDto {
  id: number = 0;
  utilizadorId: number = 0;
  tipo: string = '';
  origemId: number = 0;
  mensagem: string = '';
  utilizadorNotificacao: UtilizadorSimplificadoDto = new UtilizadorSimplificadoDto();
  utilizadorResponsavel: UtilizadorSimplificadoDto = new UtilizadorSimplificadoDto();
  lida: boolean = false;
  criadoEm : string = '';
}

export class NovaNotificacaoDto {
  utilizadorId: number = 0;
  tipo: number = 0;
  origemId: number = 0;
  mensagem: string = '';
}
