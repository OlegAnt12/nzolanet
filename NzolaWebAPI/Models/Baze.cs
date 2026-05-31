using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    [Table("tb_Baze")]
    [Index(nameof(PublicacaoId), nameof(UtilizadorId), IsUnique = true)]
    public class Baze
    {
        [Key]
        public int Id { get; set; }
        public int PublicacaoId { get; set; }

        [ForeignKey("PublicacaoId")]
        public Publicacao Publicacao { get; set; }
        public int UtilizadorId { get; set; }

        [ForeignKey("UtilizadorId")]
        public Utilizador Utilizador { get; set; }
        public EstadoBaze EstadoBaze { get; set; } = EstadoBaze.Dado;
        public DateTime DataInteracao { get; set; } = DateTime.Now;
    }
}
