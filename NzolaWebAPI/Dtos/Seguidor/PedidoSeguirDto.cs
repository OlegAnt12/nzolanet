using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.DTOs.Seguidor
{
    public class PedidoSeguirDto
    {
        public int Id { get; set; }
        public int SeguidorId { get; set; }
        public string? NomeSeguidor { get; set; }
        public string? NomeUtilizadorSeguidor { get; set; }
        public string? FotoSeguidor { get; set; }
        public int SeguidoId { get; set; }
        public string? NomeSeguido { get; set; }
        public EstadoPedido Estado { get; set; }
        public DateTime DataPedido { get; set; }
    }
}
