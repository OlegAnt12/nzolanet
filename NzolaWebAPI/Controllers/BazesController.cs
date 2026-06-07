using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Baze;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Repositories;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BazesController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IBazesRepository _bazeRepo;

        public BazesController(ContextoBDNzola contexto, IBazesRepository bazeRepo)
        {
            _contexto = contexto;
            _bazeRepo = bazeRepo;
        }

        /*[HttpGet]
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
        

        [HttpGet("{id}")]
        public async Task<IActionResult> SelecionarBaze([FromRoute] int id)
        {
            var baze = await _contexto.Bazes.FindAsync(id);

            if (baze == null)
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
        }*/

        [HttpGet("publicacao/{id}")]
        public async Task<IActionResult> GetBazesPorPublicacao([FromRoute] int id)
        {
            var bazesPublicacao = await _bazeRepo.GetBazesPorPublicacaoAsync();

            if (bazesPublicacao == null)
            {
                return NotFound();
            }

            return Ok(bazesPublicacao);
        }

        [HttpPost("{publicacaoId:int}/{utilizadorId:int}")]
        public async Task<IActionResult> DarBaze(
            [FromRoute] int publicacaoId,
            [FromRoute] int utilizadorId,
            [FromBody] DarBazeRequestDto bazeDto
        )
        {
            bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );

            if (!utilizadorExiste)
            {
                return BadRequest("Este Utilizador Não Existe");
            }

            var publicacao = await _contexto.Publicacoes.FindAsync(publicacaoId);

            if (publicacao == null)
            {
                return BadRequest("Esta Publicação Não Existe");
            }

            var bazeExistente = await _contexto.Bazes.FirstOrDefaultAsync(b =>
                b.PublicacaoId == publicacaoId && b.UtilizadorId == utilizadorId
            );

            if (bazeExistente != null)
            {
                _contexto.Bazes.Remove(bazeExistente);

                if (publicacao.QuantidadeBazes > 0)
                    publicacao.QuantidadeBazes--;

                await _contexto.SaveChangesAsync();
                return Ok(
                    new
                    {
                        mensagem = "Baze removido com sucesso!",
                        quantidadeBazes = publicacao.QuantidadeBazes,
                    }
                );
            }

            var baze = bazeDto.ParaBazeDeBazeDto(publicacaoId, utilizadorId);
            baze.DataInteracao = DateTime.Now;
            publicacao.QuantidadeBazes++;

            _contexto.Bazes.AddAsync(baze);
            _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(SelecionarBaze), new { id = baze.Id }, baze.ToBazeDto());
        }
    }
}
