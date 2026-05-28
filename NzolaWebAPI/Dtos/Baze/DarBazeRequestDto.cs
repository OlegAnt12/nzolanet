using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.Baze
{
    public class DarBazeRequestDto
    {
        public int PublicacaoId { get; set; }
        public int UtilizadorId { get; set; }
        public EstadoBaze EstadoBaze { get; set; }
        public DateTime DataInteracao { get; set; }
    }
}
