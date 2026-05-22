using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Baze
{
    public class BazeDto
    {
        public int Id {get; set;}
        public int PublicacaoId {get; set;}
        public int UtilizadorId {get; set;}
        public DateTime DataInteracao {get; set;}
    }
}