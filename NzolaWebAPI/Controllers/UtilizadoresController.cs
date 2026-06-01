using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Models;   


namespace NzolaWebAPI.Controllers{
[Route("api/[Controller]")]
[ApiController]
public class UtilizadoresController : ControllerBase{
    private readonly ContextoBDNzola _contexto;
    public UtilizadoresController(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }
       
       [HttpGet]
         public async Task<IActionResult> ListarUtilizadores()
         {
              var utilizadores = await _contexto.Utilizadores.ToListAsync()
              .Select(u => u.ToUtilizadorDto());
              return Ok(utilizadores);
         }

        [HttpGet("{id}")]

        public async Task<IActionResult> SelecionarUtilizador(int id)
        {
            var utilizador = await _contexto.Utilizadores.FindAsync(id);

            if (utilizador == null)
            {
                return NotFound();
            }

            return Ok(utilizador.ToUtilizadorDto());
        }

        [HttpPost]

        public async Task<IActionResult> CriarUtilizador([FromBody] CriarUtilizadorRequestDto utilizadorDto)
        {
            var utilizador = new Utilizador
            {
                NomeCompleto = utilizadorDto.NomeCompleto,
                Email = utilizadorDto.Email,
                Biografia = utilizadorDto.Biografia,
                Privacidade = utilizadorDto.Privacidade,
                EstadoConta = utilizadorDto.EstadoConta,
                nivelLigacao = utilizadorDto.NivelLigacao.Seguindo
            };

            _contexto.Utilizadores.Add(utilizador);
            await _contexto.SaveChangesAsync();

            return CreatedAtAction(nameof(SelecionarUtilizador), new { id = utilizador.Id }, utilizador.ToUtilizadorDto());
        }

}

}