using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Interfaces;

namespace NzolaWebAPI.Controllers
{
    /// <summary>
    /// Controlador para envio de e-mails de teste.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        /// <summary>
        /// Envia um e-mail de teste para verificar a configuração SMTP.
        /// </summary>
        /// <param name="toEmail">Endereço de e-mail de destino.</param>
        /// <returns>Mensagem de confirmação de envio.</returns>
        /// <response code="200">E-mail enviado com sucesso.</response>
        /// <response code="400">Erro ao enviar o e-mail.</response>
        /// <response code="500">Erro interno do servidor.</response>
        [HttpPost("send-test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendTestEmail(string toEmail)
        {
            try
            {
                await _emailService.SendEmailAsync(
                    toEmail,
                    "Teste NzolaNet",
                    "Olá! O serviço de e-mail está a funcionar."
                );
                return Ok("E-mail enviado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao enviar: {ex.Message}");
            }
        }
    }
}
