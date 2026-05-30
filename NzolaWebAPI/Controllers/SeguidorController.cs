using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Seguidor;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;


namespace NzolaWebAPI.Controllers
{
    [Route("api/seguidor")]
    [ApiController]
    public class SeguidorController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto; 
        public SeguidorController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
            
        }

        [HttpGet]
        public IActionResult ListarSeguidores()
        {
            var seguidores = _contexto.Seguidores.ToList()
            .Select(s => s.ToSeguidorDto());

            return Ok(seguidores);
        }

        [HttpGet("{id}")]
        public IActionResult SelecionarSeguidor([FromRoute] int id)
        {
            var seguidor = _contexto.Seguidores.Find(id);

            if (seguidor == null)
            {
                return NotFound();
            }
            return Ok(seguidor.ToSeguidorDto());
        }

        [HttpPost]
        public IActionResult Criar ([FromBody] CriarSeguidorDto criarSeguidorDto)
        {
            var seguidor = criarSeguidorDto.ToSeguidorFromCriarDto();

            _contexto.Seguidores.Add(seguidor);
            _contexto.Seguidores.SaveChanges();

            return CreatedAtAction(nameof(SelecionarSeguidor), new {id = seguidor.Id}, seguidor.ToSeguidorDto());
            
        }

        [HttpDelete]
        public IActionResult Apagar ([FromRoute] int id)
        {
            var seguidor = _contexto.Seguidores.Find(id);

            if(seguidor = null)
            {
                return NotFound();
            }

            _contexto.Seguidores.Remove(seguidor);
            _contexto.Seguidores.SaveChanges();

            return NoContent();
        }

    
        
    }
}