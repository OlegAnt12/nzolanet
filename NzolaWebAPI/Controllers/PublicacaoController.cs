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
        public IActionResult ListarPublicacoes()
        {
            var publicacoes =  _contexto.Publicacoes.ToList()
            .Select(p=>p.ToPublicacaoDto());
            return Ok(publicacoes);
        }

        [HttpGet("{id}")]
        public IActionResult SelecionarPublicacao(int id)
        {
            var publicacao =  _contexto.Publicacoes.Find(id);

            if(publicacao == null)
            {
                return NotFound();
            }
            
            return Ok(publicacao.ToPublicacaoDto());
        }
        /*
        [HttpPost]
        public IActionResult PublicarConteudo()
        {

        }
*/

    }
}