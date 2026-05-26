using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.DTOs.Comentario
{
    public class ComentarioDto
    {
        public int Id {get; set;}
        public int PublicacaoId {get; set;}
        public int UtilizadorId {get; set;}
        public string ConteudoComentario {get; set;}
        public DateTime DataComentario {get; set;} = DateTime.Now;
        public DateTime DataActualizacao {get; set;}
    }
}