import { UtilizadorSimplificadoDto } from "../utilizador/utilizadorfeed/utilizador.dto";

export class SeguidorDto {
  id: number = 0;
  seguidorId: number = 0;
  seguidoId: number = 0;
  seguidor: UtilizadorSimplificadoDto | null = null;
  seguido: UtilizadorSimplificadoDto | null = null;
  dataInicio: string = '';
}
