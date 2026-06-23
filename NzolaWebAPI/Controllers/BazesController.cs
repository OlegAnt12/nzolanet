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
using NzolaWebAPI.Services;

namespace NzolaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BazesController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IBazeRepository _bazeRepo;
        private readonly IBazeService _bazeService;

        public BazesController(
            ContextoBDNzola contexto,
            IBazeRepository bazeRepo,
            IBazeService bazeService
        )
        {
            _contexto = contexto;
            _bazeRepo = bazeRepo;
            _bazeService = bazeService;
        }

        /*[HttpGet]
        public IActionResult SelecionarBazes()
        {
            var bazes =  _contexto.Bazes.ToList();
            return Ok(bazes);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> SelecionarBaze([FromRoute] int id)
        {
            var baze = await _bazeRepo.SelecionarBazeAsync(id);

            if (baze == null)
            {
                return NotFound();
            }

            return Ok(baze.ToBazeDto());
        }

        [HttpGet("publicacao/{id}")]
        public async Task<IActionResult> GetBazesPorPublicacao([FromRoute] int id)
        {
            var bazesPublicacao = await _bazeRepo.GetBazesPorPublicacaoAsync(id);

            if (bazesPublicacao == null)
            {
                return NotFound();
            }

            var bazeDto = bazesPublicacao.Select(b => b.ToBazeDto());

            return Ok(bazeDto);
        }

        [HttpPost("{publicacaoId:int}/{utilizadorId:int}")]
        public async Task<IActionResult> DarBaze(
            [FromRoute] int publicacaoId,
            [FromRoute] int utilizadorId
        )
        {
            var resultado = await _bazeService.AlternarBazeAsync(publicacaoId, utilizadorId);

            if (resultado.ErroMensagem != null)
            {
                return BadRequest(resultado.ErroMensagem);
            }

            if (resultado.FoiRemovido)
            {
                return Ok(
                    new
                    {
                        mensagem = "Baze removido com sucesso!",
                        quantidadeBazes = resultado.QuantidadeBazes,
                    }
                );
            }

            return CreatedAtAction(
                nameof(SelecionarBaze),
                new { id = resultado.BazeDto!.Id },
                resultado.BazeDto
            );
        }
    }
}
