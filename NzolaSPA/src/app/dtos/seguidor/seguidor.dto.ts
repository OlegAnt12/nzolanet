import { UtilizadorSimplificadoDto } from "../utilizador/utilizadorfeed/utilizador.dto";

export class SeguidorDto {
  id: number=0;
  seguidor: UtilizadorSimplificadoDto=new UtilizadorSimplificadoDto();
  seguido: UtilizadorSimplificadoDto=new UtilizadorSimplificadoDto();
  dataInicio: string='';
}
