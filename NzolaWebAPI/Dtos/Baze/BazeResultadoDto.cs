using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Baze
{
    public class BazeResultadoDto
    {
        public bool FoiRemovido { get; set; }
        public int QuantidadeBazes { get; set; }
        public BazeDto? BazeDto { get; set; }
        public string? ErroMensagem { get; set; }
    }
}
