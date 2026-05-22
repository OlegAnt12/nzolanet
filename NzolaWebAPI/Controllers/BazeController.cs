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
    public class BazeController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public BazeController (ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult SelecionarBazes()
        {
            var bazes =  _contexto.Bazes.ToList();
            return Ok(bazes);
        }

        [HttpGet("utilizador/{id}")]
        public IActionResult SelecionarBazesPorUtilizador([FromRoute] int id)
        {
            /***
            *
            *
                chamar função de retorno ou verificação de utilizadores
                if(utilizador == null)
                {
                    return NotFound();
                }
            *
            *
            ***/
            var bazesUtilizador =  _contexto.Bazes.ToList().Where(b => b.UtilizadorId == id)
            .Select(b => b.ToBazeDto());
            
            return Ok(bazesUtilizador);
        }

        [HttpGet("publicacao/{id}")]
        public IActionResult GetBazesPorPublicacao([FromRoute] int id)
        {
            /***
            *
            *
                chamar função de retorno ou verificação de publicacoes
                if(publicacao == null)
                {
                    return NotFound();
                }
            *
            *
            ***/
            var bazesPublicacao =  _contexto.Bazes.ToList().Where(b => b.PublicacaoId == id)
            .Select(b => b.ToBazeDto());
            
            return Ok(bazesPublicacao);
        }
    }
}