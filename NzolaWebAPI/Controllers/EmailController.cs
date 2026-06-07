using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NzolaWebAPI.Interfaces;

namespace NzolaWebAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-test")]
        public async Task<IActionResult> SendTestEmail(string toEmail)
        {
            try
            {
                await _emailService.SendEmailAsync(toEmail, "Teste NzolaNet", "Olá! O serviço de e-mail está a funcionar.");
                return Ok("E-mail enviado com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao enviar: {ex.Message}");
            }
        }
    }
}