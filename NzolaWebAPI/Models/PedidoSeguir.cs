using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    [Table("tb_PedidosSeguir")]
    public class PedidoSeguir
    {
        [Key]
        public int Id { get; set; }

        public int SeguidorId { get; set; }

        [ForeignKey("SeguidorId")]
        public Utilizador? UtilizadorSeguidor { get; set; }

        public int SeguidoId { get; set; }

        [ForeignKey("SeguidoId")]
        public Utilizador? UtilizadorSeguido { get; set; }

        [Column(TypeName = "nvarchar(20)")]
        public EstadoPedido Estado { get; set; } = EstadoPedido.Pendente;

        public DateTime DataPedido { get; set; } = DateTime.Now;
    }
}
