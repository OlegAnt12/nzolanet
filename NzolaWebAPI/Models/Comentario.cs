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
    [Table("tb_Comentario")]
    public class Comentario
    {
        [Key]
        public int Id { get; set; }
        public int PublicacaoId { get; set; }

        [ForeignKey("PublicacaoId")]
        public Publicacao Publicacao { get; set; }
        public int UtilizadorId { get; set; }

        [ForeignKey("UtilizadorId")]
        public Utilizador Utilizador { get; set; }
        public string ConteudoComentario { get; set; } = string.Empty;
        public DateTime DataComentario { get; set; } = DateTime.Now;
        public DateTime DataActualizacao { get; set; }
    }
}
