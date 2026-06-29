using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Repositories
{
    public class PedidoSeguirRepository : IPedidoSeguirRepository
    {
        private readonly ContextoBDNzola _contexto;

        public PedidoSeguirRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task<PedidoSeguir?> ObterPorIdAsync(int id)
        {
            return await _contexto.PedidosSeguir
                .Include(p => p.UtilizadorSeguidor)
                .Include(p => p.UtilizadorSeguido)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PedidoSeguir?> ObterPendenteAsync(int seguidorId, int seguidoId)
        {
            return await _contexto.PedidosSeguir
                .FirstOrDefaultAsync(p =>
                    p.SeguidorId == seguidorId &&
                    p.SeguidoId == seguidoId &&
                    p.Estado == Models.Enums.EstadoPedido.Pendente);
        }

        public async Task<List<PedidoSeguir>> ListarPendentesPorUtilizadorAsync(int utilizadorId)
        {
            return await _contexto.PedidosSeguir
                .Include(p => p.UtilizadorSeguidor)
                .Include(p => p.UtilizadorSeguido)
                .Where(p => p.SeguidoId == utilizadorId && p.Estado == Models.Enums.EstadoPedido.Pendente)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();
        }

        public async Task<List<PedidoSeguir>> ListarPorUtilizadorAsync(int utilizadorId)
        {
            return await _contexto.PedidosSeguir
                .Include(p => p.UtilizadorSeguidor)
                .Include(p => p.UtilizadorSeguido)
                .Where(p => p.SeguidorId == utilizadorId || p.SeguidoId == utilizadorId)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();
        }

        public async Task AdicionarAsync(PedidoSeguir pedido)
        {
            await _contexto.PedidosSeguir.AddAsync(pedido);
        }

        public void Atualizar(PedidoSeguir pedido)
        {
            _contexto.PedidosSeguir.Update(pedido);
        }

        public async Task SalvarAsync()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}
