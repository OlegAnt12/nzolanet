using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace NzolaWebAPI.Models
{
    [Table("tb_Publicacoes")]
    public class Publicacao
    {
        [Key]
        public int Id { get; set; }
        public int AutorId { get; set; }

        [ForeignKey("AutorId")]
        public Utilizador Utilizador { get; set; }
        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        public DateTime DataAtualizacaoPublicacao { get; set; }
        public List<ConteudoPublicacao> Conteudos { get; set; } = new List<ConteudoPublicacao>();
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<Baze> Bazes { get; set; } = new List<Baze>();
    }
}
