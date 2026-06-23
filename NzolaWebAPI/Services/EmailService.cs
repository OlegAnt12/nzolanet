using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using NzolaWebAPI.Configurations;
using NzolaWebAPI.Interfaces;

namespace NzolaWebAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _config;

        public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration config)
        {
            _emailSettings = emailSettings.Value;
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _emailSettings.SmtpServer,
                _emailSettings.Port,
                SecureSocketOptions.StartTls
            );
            await smtp.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.SenderPassword);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        public async Task EnviarEmailConfirmacaoAsync(
            string emailDestinatario,
            string nomeUtilizador
        )
        {
            var mensagem = new MimeMessage();

            // 1. Configurar o Remetente (Nzola)
            mensagem.From.Add(
                new MailboxAddress(
                    _config["SmtpSettings:NomeEmissor"],
                    _config["SmtpSettings:EmailEmissor"]
                )
            );

            mensagem.To.Add(new MailboxAddress(nomeUtilizador, emailDestinatario));
            mensagem.Subject = "Bem-vindo à Nzola! Confirma o teu registo";

            var corpoBuilder = new BodyBuilder
            {
                HtmlBody =
                    $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee;'>
                    <h2 style='color: #4A90E2;'>Olá, {nomeUtilizador}!</h2>
                    <p>Obrigado por te registares na <strong>Nzola</strong>. A tua conta foi criada com sucesso na nossa plataforma.</p>
                    <p>Estamos muito felizes por te ter connosco. Agora já podes partilhar as tuas publicações e interagir com a comunidade (dar bazes, comentar e muito mais!).</p>
                    <br>
                    <p style='font-size: 12px; color: #888;'>Este é um e-mail automático, por favor não respondas.</p>
                </div>",
            };

            mensagem.Body = corpoBuilder.ToMessageBody();

            using var clienteSmtp = new SmtpClient();

            try
            {
                await clienteSmtp.ConnectAsync(
                    _config["SmtpSettings:Server"],
                    int.Parse(_config["SmtpSettings:Port"]!),
                    SecureSocketOptions.StartTls
                );

                // Autentica na conta de e-mail de envio
                await clienteSmtp.AuthenticateAsync(
                    _config["SmtpSettings:SenderEmail"],
                    _config["SmtpSettings:Password"]
                );

                // Envia o e-mail de forma assíncrona
                await clienteSmtp.SendAsync(mensagem);
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                // Garante que a ligação é fechada corretamente mesmo que ocorra algum erro
                await clienteSmtp.DisconnectAsync(true);
            }
        }
    }
}
