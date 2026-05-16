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
        public IActionResult GetALL()
        {
            var notificacoes =  _context.Notificacoes.ToList();
            return Ok(notificacoes);
        }
        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var Notificacao = _context.Notificacoes.Find(id);

            if(Notificacao == null)
            {
                return NotFound();
            }
            return Ok(Notificacao);
        }

       
    }

}