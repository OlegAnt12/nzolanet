using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Comentario;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador para gestão de comentários em publicações.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IComentarioService _comentarioService;

        public ComentariosController(ContextoBDNzola contexto, IComentarioService comentarioService)
        {
            _contexto = contexto;
            _comentarioService = comentarioService;
        }

        /// <summary>
        /// Lista todos os comentários de uma publicação específica.
        /// </summary>
        /// <param name="Id">ID da publicação</param>
        /// <returns>Lista de comentários da publicação</returns>
        /// <response code="200">Lista de comentários retornada com sucesso</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("publicacao/{Id}")]
        [ProducesResponseType(typeof(IEnumerable<ComentarioDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetComentarios([FromRoute] int Id)
        {
            var comentarios = await _comentarioService.ListarAsync(Id);

            return Ok(comentarios);
        }

        /// <summary>
        /// Obtém um comentário específico pelo seu ID.
        /// </summary>
        /// <param name="id">ID do comentário</param>
        /// <returns>Detalhes do comentário</returns>
        /// <response code="200">Comentário encontrado com sucesso</response>
        /// <response code="404">Comentário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ComentarioDto), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetComentario([FromRoute] int id)
        {
            var comentario = await _contexto.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound();
            }

            return Ok(comentario.ToComentarioDto());
        }

        /// <summary>
        /// Adiciona um novo comentário a uma publicação.
        /// </summary>
        /// <param name="comentarioDto">Dados do comentário a adicionar</param>
        /// <param name="publicacaoId">ID da publicação</param>
        /// <param name="utilizadorId">ID do utilizador autor</param>
        /// <returns>Comentário criado</returns>
        /// <response code="201">Comentário criado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Publicação ou utilizador não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost("{publicacaoId:int}/{utilizadorId:int}")]
        [ProducesResponseType(typeof(ComentarioDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> AdicionarComentario(
            [FromBody] AdicionarComentarioRequestDto comentarioDto,
            [FromRoute] int publicacaoId,
            int utilizadorId
        )
        {
            /*bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
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
            );*/

            var resultado = await _comentarioService.AdicionarAsync(
                publicacaoId,
                utilizadorId,
                comentarioDto
            );

            if (!resultado.Sucesso)
                return BadRequest(resultado.MensagemErro);

            return CreatedAtAction(
                nameof(GetComentario),
                new { id = resultado.ComentarioDto!.Id },
                resultado.ComentarioDto
            );
        }

        /// <summary>
        /// Edita o texto de um comentário existente.
        /// </summary>
        /// <param name="id">ID do comentário a editar</param>
        /// <param name="comentarioDto">Dados atualizados do comentário</param>
        /// <returns>Comentário atualizado</returns>
        /// <response code="200">Comentário atualizado com sucesso</response>
        /// <response code="400">Dados inválidos fornecidos</response>
        /// <response code="404">Comentário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ComentarioDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> EditarComentario(
            [FromRoute] int id,
            [FromBody] EditarComentarioRequestDto comentarioDto
        )
        {
            /*var comentario = await _contexto.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound("Comentário não encontrado");
            }

            comentario.ConteudoComentario = comentarioDto.ConteudoComentario;
            comentario.DataActualizacao = DateTime.Now;

            await _contexto.SaveChangesAsync();

            return Ok(comentario.ToComentarioDto());*/
            var resultado = await _comentarioService.EditarAsync(id, comentarioDto);

            if (resultado.NaoEncontrado)
                return NotFound(resultado.MensagemErro);

            return Ok(resultado.ComentarioDto);
        }

        /// <summary>
        /// Remove um comentário do sistema.
        /// </summary>
        /// <param name="id">ID do comentário a eliminar</param>
        /// <response code="204">Comentário eliminado com sucesso</response>
        /// <response code="404">Comentário não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExcluirComentario([FromRoute] int id)
        {
            /*var comentario = await _contexto.Comentarios.FindAsync(id);

            if (comentario == null)
            {
                return NotFound("Comentário não encontrado");
            }

            _contexto.Comentarios.Remove(comentario);
            await _contexto.SaveChangesAsync();

            return NoContent();*/

            var resultado = await _comentarioService.ExcluirAsync(id);

            if (resultado.NaoEncontrado)
                return NotFound(resultado.MensagemErro);

            return NoContent();
        }
    }
}
