using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Notificacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IUtilizadorRepository _utilizadorRepo;
        public NotificacaoController(ContextoBDNzola contexto, IUtilizadorRepository utilizadorRepo)
        {
            _contexto = contexto;
            _utilizadorRepo = utilizadorRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var notificacoes = await _contexto
                .Notificacoes.Select(notificacao => notificacao.ToNotificacaoDto())
                .ToListAsync();

            return Ok(notificacoes);
        }

        [HttpGet("utilizador/{utilizadorId}")]
        public async Task<IActionResult> Listar([FromRoute] int utilizadorId)
        {
            var notificacoes = await _contexto
                .Notificacoes.Where(n => n.UtilizadorId == utilizadorId).Include(n => n.UtilizadorResponsavel).Select(notificacao => notificacao.ToNotificacaoDto())
                .ToListAsync();

            return Ok(notificacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId([FromRoute] int id)
        {
            var notificacao = await _contexto.Notificacoes.FindAsync(id);

            if (notificacao == null)
            {
                return NotFound();
            }
            return Ok(notificacao.ToNotificacaoDto());
        }

        [HttpPost("{utilizadorId:int}")]
        public async Task<IActionResult> Criar([FromBody] CriarNotificacaoDto criarNotificacaoDto, [FromRoute] int utilizadorId)
        {
            var utilizador = await _utilizadorRepo.ObterPorIdAsync(utilizadorId);

            if (utilizador == null)
            {
                return BadRequest("Utilizador Inexistente");
            }

            var notificacao = criarNotificacaoDto.ToNotificacaoFromCriarDto(utilizadorId);

            await _contexto.Notificacoes.AddAsync(notificacao);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = notificacao.Id },
                notificacao.ToNotificacaoDto()
            );
        }

        [HttpPut]
        public async Task<IActionResult> MarcarComoLida([FromRoute] int id)
        {
            var notificacao = await _contexto.Notificacoes.FindAsync(id);

            if (notificacao == null)
            {
                return NotFound();
            }

            notificacao.Lida = true;
            await _contexto.SaveChangesAsync();

            return Ok(notificacao.ToNotificacaoDto());
        }

        [HttpDelete]
        public IActionResult Apagar([FromRoute] int id)
        {
            var notificacao = _contexto.Notificacoes.Find(id);

            if (notificacao == null)
            {
                return NotFound();
            }

            _contexto.Notificacoes.Remove(notificacao);
            _contexto.SaveChanges();

            return NoContent();
        }
    }
}
