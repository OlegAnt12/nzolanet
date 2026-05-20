using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public PublicacaoController (ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult GetPublicacoes()
        {
            var publicacoes =  _contexto.Publicacoes.ToList();
            return Ok(publicacoes);
        }

        [HttpGet("{id}")]
        public IActionResult GetPublicacao(int id)
        {
            var publicacao =  _contexto.Publicacoes.Find(id);

            if(publicacao == null)
            {
                return NotFound();
            }
            
            return Ok(publicacao);
        }
        /*
        [HttpPost]
        public IActionResult PublicarConteudo()
        {

        }
*/

    }
}