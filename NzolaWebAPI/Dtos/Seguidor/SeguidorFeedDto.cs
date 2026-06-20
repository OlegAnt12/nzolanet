using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Utilizador;

namespace NzolaWebAPI.DTOs.Seguidor
{
    public class SeguidorFeedDto
    {
        public int Id { get; set; }
        public UtilizadorSimplificadoDto Seguidor { get; set; } = null!;
        public UtilizadorSimplificadoDto Seguido { get; set; } = null!;
        public DateTime DataInicio { get; set; } 
    }
}