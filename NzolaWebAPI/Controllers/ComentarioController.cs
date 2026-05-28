using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentarioController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public ComentarioController (ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult GetComentarios()
        {
            var comentarios =  _contexto.Comentarios.ToList();
            return Ok(comentarios);
        }

        [HttpGet("{id}")]
        public IActionResult GetComentario([FromRoute] int id)
        {
            var comentario =  _contexto.Comentarios.Find(id);

            if(comentario == null)
            {
                return NotFound();
            }
            
            return Ok(comentario);
        }

        [HttpPost("publicacaoId", "utilizadorId")]
        public IActionResult AdicionarComentario ([FromBody] AdicionarComentarioRequestDto comentarioDto, [FromRoute] int publicacaoId, int utilizadorId)
        {
            bool utilizadorExiste = _contexto.Utilizadores.Any(u => u.Id ==  utilizadorId);

            if(!utilizadorExiste)
            {
                return BadRequest("Este Utilizador Não Existe");
            }

            bool publicacaoExiste = _contexto.Publicacoes.Any(p => p.Id == publicacaoId);

            if(!publicacaoExiste)
            {
                return BadRequest("Esta Publicacao Não Existe");
            }

            var comentario = comentarioDto.ParaComentarioDeComentarioDto(publicacaoId, utilizadorId);
            _contexto.Comentarios.Add(comentario);
            _contexto.Comentarios.SaveChanges();
            return CreatedAtAction(
                nameof(GetComentario),
                new { id = comentario.Id },
                comentario.ToComentarioDto()
            );
        }
    }
}