using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.DTOs.ConteudoPublicacao;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConteudoPublicacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;

        public ConteudoPublicacaoController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        [HttpGet]
        public IActionResult SelecionarConteudoPublicacoes()
        {
            var conteudoPublicacoes = _contexto
                .ConteudoPublicacoes.ToList()
                .Select(cp => cp.ToConteudoPublicacaoDto());
            return Ok(conteudoPublicacoes);
        }

        [HttpGet("{id}")]
        public IActionResult SelecionarConteudoPublicacao([FromRoute] int id)
        {
            var comentario = _contexto.ConteudoPublicacoes.Find(id);

            if (comentario == null)
            {
                return NotFound();
            }

            return Ok(comentario.ToConteudoPublicacaoDto());
        }

        [HttpPost]
        public IActionResult AdicionarConteudo(
            [FromBody] AdicionarConteudoPublicacaoRequestDto conteudoPublicacaoDto
        )
        {
            var conteudoPublicacao =
                conteudoPublicacaoDto.ParaConteudoPublicacaoDeConteudoPublicacaoDto();
            _contexto.ConteudosPublicacao.Add(conteudoPublicacao);
            _contexto.SaveChanges();

            return CreatedAtAction(
                nameof(SelecionarConteudoPublicacao),
                new(id == conteudoPublicacaoId),
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
