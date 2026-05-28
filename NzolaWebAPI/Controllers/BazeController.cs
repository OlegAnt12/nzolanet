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

        [HttpGet("{id}")]
        public IActionResult SelecionarBaze([FromRoute] int id)
        {   
            var baze =  _contexto.Bazes.Find(id);
            
            if(baze == null)
            {
                return NotFound();
            }

            return Ok(baze);
        }

        [HttpGet("utilizador/{id}")]
        public IActionResult SelecionarBazesPorUtilizador([FromRoute] int id)
        {
                chamar função de retorno ou verificação de utilizadores
                if(utilizador == null)
                {
                    return NotFound();
                }
            
            var bazesUtilizador =  _contexto.Bazes.ToList().Where(b => b.UtilizadorId == id)
            .Select(b => b.ToBazeDto());
            
            return Ok(bazesUtilizador);
        }

        [HttpGet("publicacao/{id}")]
        public IActionResult GetBazesPorPublicacao([FromRoute] int id)
        {
            var bazesPublicacao =  _contexto.Bazes.ToList().Where(b => b.PublicacaoId == id)
            .Select(b => b.ToBazeDto());

            if(bazesPublicacao == null)
                {
                    return NotFound();
                }
            
            return Ok(bazesPublicacao);
        }

        [HttpPost("publicacaoId","utilizadorId")]
        public IActionResult DarBaze([FromRoute] int publicacaoId, [FromRoute] int utilizadorId, [FromBody] DarBazeRequestDto bazeDto)
        {
            bool publicacaoExiste = _contexto.Publicacoes.Any(p => p.Id == publicacaoId);

            if(!publicacaoExiste)
            {
                return BadRequest("Esta Publicação Não Existe");
            }

            bool utilizadorExiste = _contexto.Utilizadores.Any(u => u.Id == utilizadorId)
            
            if(!utilizadorExiste)
            {
                return BadRequest("Este Utilizador Não Existe");
            }

            var baze = bazeDto.ParaBazeDeBazeDto(publicacaoId, utilizadorId);
            _contexto.Bazes.Add(baze);
            _contexto.Bazes.SaveChanges();

            return CreatedAtAction(
                nameof(SelecionarBaze),
                new { id = baze.Id },
                baze.ToBazeDto()
            );
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult ActualizarBaze ([FromBody] ActualizarBazeRequestDto bazeDto, int )
    }
}