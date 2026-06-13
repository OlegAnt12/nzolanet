using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Services
{
    public class SeguidorService : ISeguidorService
    {
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly IUtilizadorRepository _utilizadorRepository;

        public SeguidorService() { }

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

                await _seguidorRepo.SalvarAsync(); // Atualiza os contadores da publicação
                resultado.FoiRemovido = true;

                return resultado;
            }

            Seguidor seguidor = new();
            seguidor.SeguidorId = seguidorId;
            seguidor.SeguidoId = seguidoId;

            await _seguidorRepo.AdicionarAsync(baze);
            await _seguidorRepo.SalvarAsync();

            return resultado;
        }
    }
}
