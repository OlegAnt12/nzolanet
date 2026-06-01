using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudosPublicacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public ConteudosPublicacaoController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet("publicacao/{publicacaoId}")]
        public async Task<IActionResult> SelecionarConteudoPublicacoes([FromRoute] int publicacaoId)
        {
            var conteudoPublicacoes = await _contexto
                .ConteudosPublicacao
                .Where(c => c.PublicacaoId == publicacaoId)
                .Select(cp => cp.ToConteudoPublicacaoDto())
                .ToListAsync();
            return Ok(conteudoPublicacoes);
        }

        [HttpGet("{id}")]
        public IActionResult SelecionarConteudoPublicacao([FromRoute] int id)
        {
            var conteudo = _contexto.ConteudosPublicacao.Find(id);

            if (conteudo == null)
            {
                return NotFound();
            }

            return Ok(conteudo.ToConteudoPublicacaoDto());
        }

        [HttpPost("{publicacaoId}")]
        public IActionResult AdicionarConteudo(
            [FromBody] AdicionarConteudoPublicacaoRequestDto conteudoPublicacaoDto,
            int publicacaoId
        )
        {
            bool publicacaoExiste = _contexto.Publicacoes.Any(p => p.Id == publicacaoId);
            if (!publicacaoExiste)
            {
                return BadRequest("Esta publicacao Não existe");
            }

            var conteudoPublicacao =
                conteudoPublicacaoDto.ParaConteudoPublicacaoDeConteudoPublicacaoDto(publicacaoId);
            _contexto.ConteudosPublicacao.Add(conteudoPublicacao);
            _contexto.SaveChanges();

            return CreatedAtAction(
                nameof(SelecionarConteudoPublicacao),
                new{id = conteudoPublicacao.Id},
                conteudoPublicacao.ToConteudoPublicacaoDto()
            );
        }

        [HttpPut]
        [Route("{Id}")]
        public IActionResult ActualizarConteudoPublicacao(
            [FromRoute] int Id,
            [FromBody] ActualizarConteudoPublicacaoRequestDto conteudoPublicacaoDto
        )
        {
            var conteudoPublicacao = _contexto.ConteudosPublicacao.FirstOrDefault(cp =>
                cp.Id == Id
            );

            if (conteudoPublicacao == null)
            {
                return NotFound();
            }

            conteudoPublicacao.Conteudo = conteudoPublicacaoDto.Conteudo;
            conteudoPublicacao.TipoConteudo = conteudoPublicacaoDto.TipoConteudo;
            conteudoPublicacao.Ordem = conteudoPublicacaoDto.Ordem;

            _contexto.SaveChanges();
            return Ok(conteudoPublicacao.ToConteudoPublicacaoDto());
        }

        [HttpDelete]
        [Route("{Id}")]
        public IActionResult EliminarConteudoPublicacao([FromRoute] int Id)
        {
            var conteudoPublicacao = _contexto.ConteudosPublicacao.FirstOrDefault(cp =>
                cp.Id == Id
            );

            if (conteudoPublicacao == null)
            {
                return NotFound();
            }

            _contexto.ConteudosPublicacao.Remove(conteudoPublicacao);
            _contexto.SaveChanges();

            return NoContent();
        }
    }
}
