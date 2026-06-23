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
        public AutorPublicacaoDto? Seguidor { get; set; } 
        public AutorPublicacaoDto? Seguido { get; set; }
        public DateTime DataInicio { get; set; } 
    }
}