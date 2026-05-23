using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Models
{
    [Table("tb_Publicacao")]
    public class Publicacao
    {
        [Key]
        public int Id { get; set; }
        public int AutorId { get; set; }
        [ForeignKey("AutorId")]
        public Utilizador Utilizador {get; set;}
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        public List<ConteudoPublicacao> Conteudos { get; set; } = new List<ConteudoPublicacao>();
    }
}