using NzolaWebAPI.Mappers;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.Models;
using System.Linq;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacoesController : ControllerBase
    {
        private readonly ContextoBDNzola _context;

        public NotificacoesController (ContextoBDNzola context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Listar()
        {
            var notificacoes =  _context.Notificacoes
                .Select(n => n.ToNotificacaoDto()).ToList();
            return Ok(notificacoes);
        }
        [HttpGet("{id}")]
        public IActionResult BuscarPorId([FromRoute] int id)
        {
            var notificacao = _context.Notificacoes.Find(id);

            if(notificacao == null)
            {
                return NotFound();
            }
            return Ok(notificacao.ToNotificacaoDto());
        }

       
    }

}