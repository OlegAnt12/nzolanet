using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudoPublicacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public ConteudoPublicacaoController (ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult SelecionarConteudoPublicacoes()
        {
            var conteudoPublicacoes =  _contexto.ConteudoPublicacoes.ToList()
            .Select(cp => cp.ToConteudoPublicacaoDto());
            return Ok(conteudoPublicacoes);
        }

        [HttpGet("{id}")]
        public IActionResult SelecionarConteudoPublicacao([FromRoute] int id)
        {
            var comentario =  _contexto.ConteudoPublicacoes.Find(id);

            if(comentario == null)
            {
                return NotFound();
            }
            
            return Ok(comentario.ToConteudoPublicacaoDto());
        }
    }
}