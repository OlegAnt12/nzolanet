using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IPedidoSeguirRepository
    {
        Task<PedidoSeguir?> ObterPorIdAsync(int id);
        Task<PedidoSeguir?> ObterPendenteAsync(int seguidorId, int seguidoId);
        Task<List<PedidoSeguir>> ListarPendentesPorUtilizadorAsync(int utilizadorId);
        Task<List<PedidoSeguir>> ListarPorUtilizadorAsync(int utilizadorId);
        Task AdicionarAsync(PedidoSeguir pedido);
        void Atualizar(PedidoSeguir pedido);
        Task SalvarAsync();
    }
}
