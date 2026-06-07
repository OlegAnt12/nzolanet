using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Comentario;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public ComentariosController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet("publicacao/{Id}")]
        public async Task<IActionResult> GetComentarios([FromRoute] int Id)
        {
            var comentarios = await _contexto
                .Comentarios.Where(b => b.PublicacaoId == Id)
                .Select(c => c.ToComentarioDto())
                .ToListAsync();

            return Ok(comentarios);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComentario([FromRoute] int id)
        {
            var comentario = await _contexto.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound();
            }

            return Ok(comentario);
        }

        [HttpPost("{publicacaoId:int}/{utilizadorId:int}")]
        public async Task<IActionResult> AdicionarComentario(
            [FromBody] AdicionarComentarioRequestDto comentarioDto,
            [FromRoute] int publicacaoId,
            int utilizadorId
        )
        {
            bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );

            if (!utilizadorExiste)
            {
                return BadRequest("Este Utilizador Não Existe");
            }

            bool publicacaoExiste = await _contexto.Publicacoes.AnyAsync(p => p.Id == publicacaoId);

            if (!publicacaoExiste)
            {
                return BadRequest("Esta Publicacao Não Existe");
            }

            var comentario = comentarioDto.ParaComentarioDeComentarioDto(
                publicacaoId,
                utilizadorId
            );

            comentario.DataComentario = DateTime.Now;

            await _contexto.Comentarios.AddAsync(comentario);
            await _contexto.SaveChangesAsync();
            return CreatedAtAction(
                nameof(GetComentario),
                new { id = comentario.Id },
                comentario.ToComentarioDto()
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> EditarComentario(
            [FromRoute] int id,
            [FromBody] EditarComentarioRequestDto comentarioDto
        )
        {
            var comentario = await _contexto.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound("Comentário não encontrado");
            }

            comentario.ConteudoComentario = comentarioDto.ConteudoComentario;
            comentario.DataActualizacao = DateTime.Now;

            await _contexto.SaveChangesAsync();

            return Ok(comentario.ToComentarioDto());
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> ExcluirComentario([FromRoute] int id)
        {
            var comentario = await _contexto.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound("Comentário não encontrado");
            }

            _contexto.Comentarios.Remove(comentario);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}
