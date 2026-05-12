using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Models
{
    public class Publicacao
    {
        public int Id { get; set; }
        public int AutorId { get; set; }
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataPublicacao { get; set; }
        public List<ConteudoPublicacao> Conteudos { get; set; } = new List<ConteudoPublicacao>();
    }
}