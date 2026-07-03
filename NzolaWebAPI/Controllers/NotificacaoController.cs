using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Notificacao;
using NzolaWebAPI.Hubs;
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
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificacaoController(
            ContextoBDNzola contexto,
            IUtilizadorRepository utilizadorRepo,
            IHubContext<NotificationHub> hubContext)
        {
            _contexto = contexto;
            _utilizadorRepo = utilizadorRepo;
            _hubContext = hubContext;
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
                return NotFound();

            return Ok(notificacao.ToNotificacaoDto());
        }

        [HttpPost("{utilizadorId:int}")]
        public async Task<IActionResult> Criar([FromBody] CriarNotificacaoDto criarNotificacaoDto, [FromRoute] int utilizadorId)
        {
            var utilizador = await _utilizadorRepo.ObterPorIdAsync(utilizadorId);

            if (utilizador == null)
                return BadRequest("Utilizador Inexistente");

            var notificacao = criarNotificacaoDto.ToNotificacaoFromCriarDto(utilizadorId);

            await _contexto.Notificacoes.AddAsync(notificacao);
            await _contexto.SaveChangesAsync();

            var dto = notificacao.ToNotificacaoDto();

            await _hubContext.Clients.Group($"user_{utilizadorId}").SendAsync("ReceberNotificacao", dto);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = notificacao.Id },
                dto
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> MarcarComoLida([FromRoute] int id)
        {
            var notificacao = await _contexto.Notificacoes.FindAsync(id);

            if (notificacao == null)
                return NotFound();

            notificacao.Lida = true;
            await _contexto.SaveChangesAsync();

            return Ok(notificacao.ToNotificacaoDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Apagar([FromRoute] int id)
        {
            var notificacao = await _contexto.Notificacoes.FindAsync(id);

            if (notificacao == null)
                return NotFound();

            _contexto.Notificacoes.Remove(notificacao);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}
