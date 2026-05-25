using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Publicacao;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public PublicacaoController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult ListarPublicacoes()
        {
            var publicacoes = _contexto.Publicacoes.ToList().Select(p => p.ToPublicacaoDto());
            return Ok(publicacoes);
        }

        [HttpGet("{id}")]
        public IActionResult SelecionarPublicacao(int id)
        {
            var publicacao = _contexto.Publicacoes.Find(id);

            if (publicacao == null)
            {
                return NotFound();
            }

            return Ok(publicacao.ToPublicacaoDto());
        }

        [HttpPost("{utilizadorId}")]
        public IActionResult PublicarConteudo(
            [FromRoute] int utilizadorId,
            [FromBody] CriarPublicacaoRequestDto publicacaoDto
        )
        {
            utilizadorExiste = _contexto.Utilizadores.AnyAsync(u => u.Id == utilizadorId);
            if (!utilizadorExiste)
            {
                return BadRequest("Este Utilizador não existente");
            }

            var publicacao = publicacaoDto.ParaPublicacaoDePublicacaoDto(utilizadorId);
            _contexto.Publicacoes.Add(publicacao);
            _contexto.Publicacoes.SaveChanges();
            return CreatedAtAction(
                nameof(SelecionarPublicacao),
                new { id = publicacao.Id },
                publicacao.ToPublicacaoDto()
            );
        }

        [HttpPut]
        [Route("{Id}")]
        public IActionResult ActualizarPublicacao(
            [FromRoute] int Id,
            [FromBody] ActualizarPublicacaoRequestDto putPublicacaoDto
        )
        {
            var publicacao = _contexto.Publicacoes.FirstOrDefault(p => p.Id == Id);

            if (publicacao == null)
            {
                return NotFound();
            }

            publicacao.QuantidadeBazes = putPublicacaoDto.QuantidadeBazes;
            publicacao.QuantidadeComentarios = putPublicacaoDto.QuantidadeComentarios;
            publicacao.DataAtualizacaoPublicacao = DateTime.Now;

            contexto.SaveChanges();

            return Ok(publicacao.ToPublicacaoDto());
        }

        [HttpDelete]
        [Route("{Id}")]
        public IActionResult EliminarPublicacao([FromRoute] int Id)
        {
            var publicacao = _contexto.Publicacoes.FirstOrDefault(p => p.Id == Id);

            if (publicacao == null)
            {
                return NotFound();
            }

            _contexto.Publicacoes.Remove(publicacao);
            _contexto.SaveChanges();

            return NoContent();
        }
    }
}
