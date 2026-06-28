using NzolaWebAPI.DTOs.Seguidor;

namespace NzolaWebAPI.Interfaces
{
    public interface IPedidoSeguirService
    {
        Task<PedidoSeguirDto?> SolicitarSeguimentoAsync(int seguidorId, int seguidoId);
        Task<List<PedidoSeguirDto>> ListarPendentesAsync(int utilizadorId);
        Task<PedidoSeguirDto?> AceitarPedidoAsync(int pedidoId);
        Task<PedidoSeguirDto?> RejeitarPedidoAsync(int pedidoId);
    }
}
