using NzolaWebAPI.DTOs.Seguidor;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Services
{
    public class SeguidorService : ISeguidorService
    {
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly IUtilizadorRepository _utilizadorRepository;
        private readonly IPedidoSeguirRepository _pedidoSeguirRepo;

        public SeguidorService(
            ISeguidorRepository seguidorRepo,
            IUtilizadorRepository utilizadorRepository,
            IPedidoSeguirRepository pedidoSeguirRepo
        )
        {
            _seguidorRepo = seguidorRepo;
            _utilizadorRepository = utilizadorRepository;
            _pedidoSeguirRepo = pedidoSeguirRepo;
        }

        public async Task<SeguirResultadoDto> AlternarSeguirAsync(int seguidorId, int seguidoId)
        {
            var resultado = new SeguirResultadoDto();
            var seguidorExiste = await _utilizadorRepository.ObterPorIdAsync(seguidorId);
            if (seguidorExiste == null)
            {
                resultado.ErroMensagem = "Este Utilizador Não Existe";
                return resultado;
            }

            var seguidoExiste = await _utilizadorRepository.ObterPorIdAsync(seguidoId);
            if (seguidoExiste == null)
            {
                resultado.ErroMensagem = "Este Utilizador Não Existe";
                return resultado;
            }

            var relacaoExistente = await _seguidorRepo.ObterPorRelacaoAsync(seguidorId, seguidoId);

            if (relacaoExistente != null)
            {
                _seguidorRepo.Remover(relacaoExistente);
                await _seguidorRepo.SalvarAsync();
                resultado.FoiRemovido = true;
                return resultado;
            }

            if (seguidoExiste.Privacidade == EstadoAcesso.Privado)
            {
                var pedidoPendente = await _pedidoSeguirRepo.ObterPendenteAsync(seguidorId, seguidoId);
                if (pedidoPendente != null)
                {
                    resultado.ErroMensagem = "Já existe um pedido de seguimento pendente.";
                    return resultado;
                }

                resultado.ErroMensagem = "PRIVADO_PEDIDO_ENVIADO";
                return resultado;
            }

            Seguidor seguidor = new()
            {
                SeguidorId = seguidorId,
                SeguidoId = seguidoId
            };

            await _seguidorRepo.AdicionarAsync(seguidor);
            await _seguidorRepo.SalvarAsync();

            resultado.SeguidorDto = seguidor.ToSeguidorDto();

            return resultado;
        }
    }
}
