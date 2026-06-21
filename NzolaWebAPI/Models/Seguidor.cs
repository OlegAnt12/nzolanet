using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks;

namespace NzolaWebAPI.Models
{
    [Table("tb_Seguidores")]
    public class Seguidor
    {
        [Key]
        public int Id { get; set; }

        public int SeguidorId { get; set; }

        [ForeignKey("SeguidorId")]
        public Utilizador? UtilizadorSeguidor { get; set; }

        public int SeguidoId { get; set; }

        [ForeignKey("SeguidoId")]
        public Utilizador? UtilizadorSeguido { get; set; }

        public DateTime DataInicio { get; set; } = DateTime.Now;
    }
}
