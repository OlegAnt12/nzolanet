using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Baze;
using NzolaWebAPI.Hubs;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Repositories;
using NzolaWebAPI.Services;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador de bazes (likes/reactions) em publicações.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BazesController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IBazeRepository _bazeRepo;
        private readonly IBazeService _bazeService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public BazesController(
            ContextoBDNzola contexto,
            IBazeRepository bazeRepo,
            IBazeService bazeService,
            IHubContext<NotificationHub> hubContext
        )
        {
            _contexto = contexto;
            _bazeRepo = bazeRepo;
            _bazeService = bazeService;
            _hubContext = hubContext;
        }

        /*[HttpGet]
        public IActionResult SelecionarBazes()
        {
            var bazes =  _contexto.Bazes.ToList();
            return Ok(bazes);
        }

        [HttpGet("utilizador/{id}")]
        public IActionResult SelecionarBazesPorUtilizador([FromRoute] int id)
        {
                chamar função de retorno ou verificação de utilizadores
                if(utilizador == null)
                {
                    return NotFound();
                }
            
            var bazesUtilizador =  _contexto.Bazes.ToList().Where(b => b.UtilizadorId == id)
            .Select(b => b.ToBazeDto());
            
            return Ok(bazesUtilizador);
        }*/

        /// <summary>
        /// Obtém os detalhes de um baze específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do baze</param>
        /// <returns>Detalhes do baze</returns>
        /// <response code="200">Baze encontrado com sucesso</response>
        /// <response code="404">Baze não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BazeDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> SelecionarBaze([FromRoute] int id)
        {
            var baze = await _bazeRepo.SelecionarBazeAsync(id);

            if (baze == null)
            {
                return NotFound();
            }

            return Ok(baze.ToBazeDto());
        }

        /// <summary>
        /// Obtém a lista de bazes de uma publicação específica.
        /// </summary>
        /// <param name="id">ID da publicação</param>
        /// <returns>Lista de bazes da publicação</returns>
        /// <response code="200">Lista de bazes retornada com sucesso</response>
        /// <response code="404">Publicação não encontrada</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("publicacao/{id}")]
        [ProducesResponseType(typeof(IEnumerable<BazeDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBazesPorPublicacao([FromRoute] int id)
        {
            var bazesPublicacao = await _bazeRepo.GetBazesPorPublicacaoAsync(id);

            if (bazesPublicacao == null)
            {
                return NotFound();
            }

            var bazeDto = bazesPublicacao.Select(b => b.ToBazeDto());

            return Ok(bazeDto);
        }

        /// <summary>
        /// Adiciona ou remove um baze (like) a uma publicação. Se o utilizador já deu baze, remove-o (toggle).
        /// </summary>
        /// <param name="publicacaoId">ID da publicação</param>
        /// <param name="utilizadorId">ID do utilizador</param>
        /// <returns>Resultado da operação (adicionado/removido)</returns>
        /// <response code="200">Baze removido com sucesso</response>
        /// <response code="201">Baze adicionado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{publicacaoId:int}/{utilizadorId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DarBaze(
            [FromRoute] int publicacaoId,
            [FromRoute] int utilizadorId
        )
        {
            var resultado = await _bazeService.AlternarBazeAsync(publicacaoId, utilizadorId);

            if (resultado.ErroMensagem != null)
            {
                return BadRequest(resultado.ErroMensagem);
            }

            await _hubContext.Clients.All.SendAsync("AtualizarBaze", new
            {
                publicacaoId,
                quantidadeBazes = resultado.QuantidadeBazes,
                jaDeuBaze = !resultado.FoiRemovido,
            });

            if (resultado.FoiRemovido)
            {
                return Ok(
                    new
                    {
                        mensagem = "Baze removido com sucesso!",
                        quantidadeBazes = resultado.QuantidadeBazes,
                    }
                );
            }

            return CreatedAtAction(
                nameof(SelecionarBaze),
                new { id = resultado.BazeDto!.Id },
                resultado.BazeDto
            );
        }
    }
}
