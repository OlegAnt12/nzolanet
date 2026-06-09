using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces; 
using NzolaWebAPI.Mappers;

namespace NzolaWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacaoController : ControllerBase
    {
        private readonly ContextoBDNzola _contexto;
        private readonly ITokenService _tokenService; 

       
        public AutenticacaoController(ContextoBDNzola contexto, ITokenService tokenService)
        {
            _contexto = contexto;
            _tokenService = tokenService;
        }

        [HttpPost("registar")]
        public async Task<IActionResult> Registar([FromBody] CriarUtilizadorRequestDto registoDto)
        {
            var emailExiste = await _contexto.Utilizadores
                .AnyAsync(u => u.Email.ToLower() == registoDto.Email.ToLower());

            if (emailExiste)
            {
                return BadRequest("Este e-mail já está a ser utilizado");
            }

            var novoUtilizador = registoDto.ToUtilizadorFromCriarDto();
            novoUtilizador.DataRegistro = DateTime.UtcNow;

            _contexto.Utilizadores.Add(novoUtilizador);
            await _contexto.SaveChangesAsync();

            return Ok(new { mensagem = "Utilizador registado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var utilizador = await _contexto.Utilizadores
                .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

            if (utilizador == null || utilizador.PalavraPasse != loginDto.PalavraPasse)
            {
                return Unauthorized("E-mail ou palavra-passe incorretos");
            }


            var tokenGerado = _tokenService.CriarToken(utilizador);

           
            return Ok(new {
                mensagem = "Login efetuado com sucesso!",
                token = tokenGerado,
                utilizador = utilizador.ToUtilizadorDto()
            });
        }
    }
}