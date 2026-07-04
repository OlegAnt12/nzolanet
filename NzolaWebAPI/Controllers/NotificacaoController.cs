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
    /// <summary>
    /// Controlador para gestão de notificações dos utilizadores.
    /// </summary>
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

        /// <summary>
        /// Lista todas as notificações do sistema.
        /// </summary>
        /// <returns>Lista de notificações</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificacaoDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Listar()
        {
            var notificacoes = await _contexto
                .Notificacoes.Select(notificacao => notificacao.ToNotificacaoDto())
                .ToListAsync();

            return Ok(notificacoes);
        }

        /// <summary>
        /// Lista as notificações de um utilizador específico.
        /// </summary>
        /// <param name="utilizadorId">ID do utilizador</param>
        /// <returns>Lista de notificações do utilizador</returns>
        [HttpGet("utilizador/{utilizadorId}")]
        [ProducesResponseType(typeof(IEnumerable<NotificacaoDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Listar([FromRoute] int utilizadorId)
        {
            var notificacoes = await _contexto
                .Notificacoes.Where(n => n.UtilizadorId == utilizadorId).Include(n => n.UtilizadorResponsavel).Select(notificacao => notificacao.ToNotificacaoDto())
                .ToListAsync();

            return Ok(notificacoes);
        }

        /// <summary>
        /// Obtém uma notificação pelo seu ID.
        /// </summary>
        /// <param name="id">ID da notificação</param>
        /// <returns>Notificação encontrada</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificacaoDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> BuscarPorId([FromRoute] int id)
        {
            var notificacao = await _contexto.Notificacoes.FindAsync(id);

            if (notificacao == null)
                return NotFound();

            return Ok(notificacao.ToNotificacaoDto());
        }

        /// <summary>
        /// Cria uma nova notificação para um utilizador.
        /// </summary>
        /// <param name="criarNotificacaoDto">Dados da notificação</param>
        /// <param name="utilizadorId">ID do utilizador destinatário</param>
        /// <returns>Notificação criada</returns>
        [HttpPost("{utilizadorId:int}")]
        [ProducesResponseType(typeof(NotificacaoDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
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

        /// <summary>
        /// Marca uma notificação como lida.
        /// </summary>
        /// <param name="id">ID da notificação</param>
        /// <returns>Notificação atualizada</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NotificacaoDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> MarcarComoLida([FromRoute] int id)
        {
            var notificacao = await _contexto.Notificacoes.FindAsync(id);

            if (notificacao == null)
                return NotFound();

            notificacao.Lida = true;
            await _contexto.SaveChangesAsync();

            return Ok(notificacao.ToNotificacaoDto());
        }

        /// <summary>
        /// Remove uma notificação do sistema.
        /// </summary>
        /// <param name="id">ID da notificação</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
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
