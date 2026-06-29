using NzolaWebAPI.DTOs.Seguidor;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Services
{
    public class PedidoSeguirService : IPedidoSeguirService
    {
        private readonly IPedidoSeguirRepository _pedidoRepo;
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly IUtilizadorRepository _utilizadorRepo;

        public PedidoSeguirService(
            IPedidoSeguirRepository pedidoRepo,
            ISeguidorRepository seguidorRepo,
            IUtilizadorRepository utilizadorRepo)
        {
            _pedidoRepo = pedidoRepo;
            _seguidorRepo = seguidorRepo;
            _utilizadorRepo = utilizadorRepo;
        }

        public async Task<PedidoSeguirDto?> SolicitarSeguimentoAsync(int seguidorId, int seguidoId)
        {
            var seguidor = await _utilizadorRepo.ObterPorIdAsync(seguidorId);
            var seguido = await _utilizadorRepo.ObterPorIdAsync(seguidoId);

            if (seguidor == null || seguido == null) return null;

            var relacaoExistente = await _seguidorRepo.ObterPorRelacaoAsync(seguidorId, seguidoId);
            if (relacaoExistente != null) return null;

            var pedidoExistente = await _pedidoRepo.ObterPendenteAsync(seguidorId, seguidoId);
            if (pedidoExistente != null) return null;

            var pedido = new PedidoSeguir
            {
                SeguidorId = seguidorId,
                SeguidoId = seguidoId,
                Estado = EstadoPedido.Pendente,
                DataPedido = DateTime.Now
            };

            await _pedidoRepo.AdicionarAsync(pedido);
            await _pedidoRepo.SalvarAsync();

            return new PedidoSeguirDto
            {
                Id = pedido.Id,
                SeguidorId = pedido.SeguidorId,
                NomeSeguidor = seguidor.NomeCompleto,
                NomeUtilizadorSeguidor = seguidor.NomeUtilizador,
                FotoSeguidor = seguidor.FotoPerfil != null ? Convert.ToBase64String(seguidor.FotoPerfil) : null,
                SeguidoId = pedido.SeguidoId,
                NomeSeguido = seguido.NomeCompleto,
                Estado = pedido.Estado,
                DataPedido = pedido.DataPedido
            };
        }

        public async Task<List<PedidoSeguirDto>> ListarPendentesAsync(int utilizadorId)
        {
            var pedidos = await _pedidoRepo.ListarPendentesPorUtilizadorAsync(utilizadorId);
            return pedidos.Select(p => new PedidoSeguirDto
            {
                Id = p.Id,
                SeguidorId = p.SeguidorId,
                NomeSeguidor = p.UtilizadorSeguidor?.NomeCompleto,
                NomeUtilizadorSeguidor = p.UtilizadorSeguidor?.NomeUtilizador,
                FotoSeguidor = p.UtilizadorSeguidor?.FotoPerfil != null
                    ? Convert.ToBase64String(p.UtilizadorSeguidor.FotoPerfil) : null,
                SeguidoId = p.SeguidoId,
                NomeSeguido = p.UtilizadorSeguido?.NomeCompleto,
                Estado = p.Estado,
                DataPedido = p.DataPedido
            }).ToList();
        }

        public async Task<PedidoSeguirDto?> AceitarPedidoAsync(int pedidoId)
        {
            var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId);
            if (pedido == null || pedido.Estado != EstadoPedido.Pendente) return null;

            pedido.Estado = EstadoPedido.Aceite;
            _pedidoRepo.Atualizar(pedido);

            var seguidor = new Seguidor
            {
                SeguidorId = pedido.SeguidorId,
                SeguidoId = pedido.SeguidoId,
                DataInicio = DateTime.Now
            };
            await _seguidorRepo.AdicionarAsync(seguidor);
            await _pedidoRepo.SalvarAsync();

            return new PedidoSeguirDto
            {
                Id = pedido.Id,
                SeguidorId = pedido.SeguidorId,
                NomeSeguidor = pedido.UtilizadorSeguidor?.NomeCompleto,
                NomeUtilizadorSeguidor = pedido.UtilizadorSeguidor?.NomeUtilizador,
                FotoSeguidor = pedido.UtilizadorSeguidor?.FotoPerfil != null
                    ? Convert.ToBase64String(pedido.UtilizadorSeguidor.FotoPerfil) : null,
                SeguidoId = pedido.SeguidoId,
                NomeSeguido = pedido.UtilizadorSeguido?.NomeCompleto,
                Estado = pedido.Estado,
                DataPedido = pedido.DataPedido
            };
        }

        public async Task<PedidoSeguirDto?> RejeitarPedidoAsync(int pedidoId)
        {
            var pedido = await _pedidoRepo.ObterPorIdAsync(pedidoId);
            if (pedido == null || pedido.Estado != EstadoPedido.Pendente) return null;

            pedido.Estado = EstadoPedido.Rejeitado;
            _pedidoRepo.Atualizar(pedido);
            await _pedidoRepo.SalvarAsync();

            return new PedidoSeguirDto
            {
                Id = pedido.Id,
                SeguidorId = pedido.SeguidorId,
                NomeSeguidor = pedido.UtilizadorSeguidor?.NomeCompleto,
                NomeUtilizadorSeguidor = pedido.UtilizadorSeguidor?.NomeUtilizador,
                FotoSeguidor = pedido.UtilizadorSeguidor?.FotoPerfil != null
                    ? Convert.ToBase64String(pedido.UtilizadorSeguidor.FotoPerfil) : null,
                SeguidoId = pedido.SeguidoId,
                NomeSeguido = pedido.UtilizadorSeguido?.NomeCompleto,
                Estado = pedido.Estado,
                DataPedido = pedido.DataPedido
            };
        }
    }
}
