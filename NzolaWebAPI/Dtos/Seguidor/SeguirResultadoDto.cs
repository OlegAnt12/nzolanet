using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Seguidor
{
    public class SeguirResultadoDto
    {
        public bool FoiRemovido { get; set; }
        public SeguidorDto? SeguidorDto { get; set; }
        public string? ErroMensagem { get; set; }
        
    }
}
