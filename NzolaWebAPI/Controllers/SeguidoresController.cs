using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Baze;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Services;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador para gestão de relações de seguimento entre utilizadores.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SeguidoresController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly ISeguidorRepository _seguidorRepo;
        private readonly ISeguidorService _seguidorService;

        public SeguidoresController(
            ContextoBDNzola contexto,
            ISeguidorRepository seguidorRepo,
            ISeguidorService seguidorService
        )
        {
            _contexto = contexto;
            _seguidorRepo = seguidorRepo;
            _seguidorService = seguidorService;
        }

        /// <summary>
        /// Lista todos os seguidores de um utilizador.
        /// </summary>
        /// <param name="utillizadorId">ID do utilizador</param>
        /// <returns>Lista de seguidores</returns>
        /// <response code="200">Seguidores retornados com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("utilizador/{utillizadorId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarSeguidoresPorUtilizador(
            [FromRoute] int utillizadorId
        )
        {
            var seguidores = await _seguidorRepo.ListarSeguidoresPorUtilizadorAsync(utillizadorId);
            return Ok(seguidores.Select(s => s.ToSeguidorFeedDto()));
        }

        /// <summary>
        /// Verifica se um utilizador segue outro.
        /// </summary>
        /// <param name="seguidorId">ID do potencial seguidor</param>
        /// <param name="seguidoId">ID do utilizador seguido</param>
        /// <returns>Relação de seguimento</returns>
        /// <response code="200">Verificação realizada com sucesso</response>
        /// <response code="400">Parâmetros inválidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("verificar/{seguidorId}/{seguidoId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> verificarRelacaoSeguidor([FromRoute] int seguidorId, [FromRoute] int seguidoId)
        {
            var relacao = _seguidorRepo.ObterPorRelacaoAsync(seguidorId, seguidoId);
            return Ok(relacao);
        }

        /// <summary>
        /// Obtém os detalhes de uma relação de seguimento específica.
        /// </summary>
        /// <param name="relacaoId">ID da relação de seguimento</param>
        /// <returns>Detalhes da relação de seguimento</returns>
        /// <response code="200">Relação encontrada</response>
        /// <response code="404">Relação não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{relacaoId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SelecionarSeguidor([FromRoute] int relacaoId)
        {
            var seguidor = await _seguidorRepo.SelecionarRelacaoIdAsync(relacaoId);

            if (seguidor == null)
            {
                return NotFound();
            }

            return Ok(seguidor.ToSeguidorFeedDto());
        }

        /// <summary>
        /// Segue ou deixa de seguir um utilizador (toggle). Funciona tanto para perfis públicos como privados.
        /// </summary>
        /// <param name="seguidorId">ID do seguidor</param>
        /// <param name="seguidoId">ID do utilizador a seguir/deixar de seguir</param>
        /// <returns>Resultado da operação indicando se está a seguir ou não</returns>
        /// <response code="200">Operação realizada com sucesso</response>
        /// <response code="201">Relação de seguimento criada</response>
        /// <response code="400">Erro na operação</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{seguidorId}/{seguidoId}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AlternarSeguir(
            [FromRoute] int seguidorId,
            [FromRoute] int seguidoId
        )
        {
            try
            {
                var seguidor = await _seguidorService.AlternarSeguirAsync(seguidorId, seguidoId);

                if (!string.IsNullOrEmpty(seguidor.ErroMensagem))
                {
                    return BadRequest(seguidor.ErroMensagem);
                }

                if (seguidor.FoiRemovido)
                {
                    return Ok(
                        new { mensagem = "Relação removida com sucesso!", estaSeguindo = false }
                    );
                }

                return Ok(new { mensagem = "Relação criada com sucesso!", estaSeguindo = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro em AlternarSeguir: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }

        /// <summary>
        /// Lista os utilizadores que um determinado utilizador segue.
        /// </summary>
        /// <param name="utilizadorId">ID do utilizador</param>
        /// <returns>Lista de utilizadores seguidos</returns>
        /// <response code="200">Lista retornada com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("seguindo/{utilizadorId:int}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ListarSeguindo(int utilizadorId)
        {
            var seguindo = await _seguidorRepo.ListarSeguindoAsync(utilizadorId);
            var ids = seguindo.Select(s => s.ToSeguidorFeedDto());

            return Ok(ids);
        }
    }
}
