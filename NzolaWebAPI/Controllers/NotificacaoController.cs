using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Notificacao;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public NotificacaoController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var notificacoes = _contexto
                .Notificacoes.Select(notificacao => notificacao.ToNotificacaoDto())
                .ToList();

            return Ok(notificacoes);
        }

        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            var notificacao = _contexto.Notificacoes.Find(id);

            if (notificacao == null)
            {
                return NotFound();
            }
            return Ok(notificacao.ToNotificacaoDto());
        }

        [HttpPost]
        public IActionResult Criar([FromBody] CriarNotificacaoDto criarNotificacaoDto)
        {
            var notificacao = criarNotificacaoDto.ToNotificacaoFromCriarDto();

            _contexto.Notificacoes.Add(notificacao);
            _contexto.SaveChanges();

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = notificacao.Id },
                notificacao.ToNotificacaoDto()
            );
        }

        [HttpPut]
        public IActionResult MarcarComoLida([FromRoute] int id)
        {
            var notificacao = _contexto.Notificacoes.Find(id);

            if (notificacao == null)
            {
                return NotFound();
            }

            notificacao.Lida = true;
           _contexto.SaveChanges();

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
