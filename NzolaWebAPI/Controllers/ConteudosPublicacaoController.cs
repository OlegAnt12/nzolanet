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
                .ConteudosPublicacao.Where(c => c.PublicacaoId == publicacaoId)
                .Select(cp => cp.ToConteudoPublicacaoDto())
                .ToListAsync();
            return Ok(conteudoPublicacoes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SelecionarConteudoPublicacao([FromRoute] int id)
        {
            var conteudo = await _contexto.ConteudosPublicacao.FindAsync(id);

            if (conteudo == null)
            {
                return NotFound();
            }

            return Ok(conteudo.ToConteudoPublicacaoDto());
        }

        [HttpPost("{publicacaoId}")]
        public async Task<IActionResult> AdicionarConteudo(
            [FromForm] AdicionarConteudoPublicacaoRequestDto conteudoPublicacaoDto,
            int publicacaoId
        )
        {
            bool publicacaoExiste = await _contexto.Publicacoes.AnyAsync(p => p.Id == publicacaoId);
            if (!publicacaoExiste)
            {
                return BadRequest("Esta publicacao Não existe");
            }

            var conteudoPublicacao =
                conteudoPublicacaoDto.ParaConteudoPublicacaoDeConteudoPublicacaoDto(publicacaoId);

            if (
                conteudoPublicacaoDto.TipoConteudo == "Imagem"
                || conteudoPublicacaoDto.TipoConteudo == "Video"
            )
            {
                if (
                    conteudoPublicacaoDto.Conteudo == null
                    || conteudoPublicacaoDto.Conteudo.Length == 0
                )
                {
                    return BadRequest("Ficheiro Multimédia em falta.");
                }

                var nomeConteudo =
                    Guid.NewGuid().ToString()
                    + PathGetExtension(conteudoPublicacaoDto.Conteudo.FileName);
                var caminho = Path.Combine("wwwroot/uploads", nomeConteudo);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await conteudoPublicacaoDto.Conteudo.CopyToAsync(stream);
                }

                conteudoPublicacao.Conteudo = $"/uploads/{nomeConteudo}";
            }
            else
            {
                conteudoPublicacao.Conteudo = conteudoPublicacaoDto.Conteudo;
            }

            await _contexto.ConteudosPublicacao.AddAsync(conteudoPublicacao);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(
                nameof(SelecionarConteudoPublicacao),
                new { id = conteudoPublicacao.Id },
                conteudoPublicacao.ToConteudoPublicacaoDto()
            );
        }

        [HttpPut]
        [Route("{Id}")]
        public async Task<IActionResult> ActualizarConteudoPublicacao(
            [FromRoute] int Id,
            [FromBody] ActualizarConteudoPublicacaoRequestDto conteudoPublicacaoDto
        )
        {
            var conteudoPublicacao = await _contexto.ConteudosPublicacao.FirstOrDefaultAsync(cp =>
                cp.Id == Id
            );

            if (conteudoPublicacao == null)
            {
                return NotFound();
            }

            conteudoPublicacao.Conteudo = conteudoPublicacaoDto.Conteudo;
            conteudoPublicacao.TipoConteudo = conteudoPublicacaoDto.TipoConteudo;
            conteudoPublicacao.Ordem = conteudoPublicacaoDto.Ordem;

            await _contexto.SaveChangesAsync();
            return Ok(conteudoPublicacao.ToConteudoPublicacaoDto());
        }

        [HttpDelete]
        [Route("{Id}")]
        public async Task<IActionResult> EliminarConteudoPublicacao([FromRoute] int Id)
        {
            var conteudoPublicacao = await _contexto.ConteudosPublicacao.FirstOrDefault(cp =>
                cp.Id == Id
            );

            if (conteudoPublicacao == null)
            {
                return NotFound();
            }

            _contexto.ConteudosPublicacao.Remove(conteudoPublicacao);
            await _contexto.SaveChangesAsync();

            return NoContent();
        }
    }
}
