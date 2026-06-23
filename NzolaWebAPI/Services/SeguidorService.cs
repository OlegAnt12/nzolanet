using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
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

        public SeguidorService(
            ISeguidorRepository seguidorRepo,
            IUtilizadorRepository utilizadorRepository
        )
        {
            _seguidorRepo = seguidorRepo;
            _utilizadorRepository = utilizadorRepository;
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

                await _seguidorRepo.SalvarAsync(); // Atualiza os contadores da publicação
                resultado.FoiRemovido = true;

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
