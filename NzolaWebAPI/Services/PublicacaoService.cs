using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Services
{
    public class PublicacaoService : IPublicacaoService
    {
        private readonly IPublicacaoRepository _publicacaoRepo;
        private readonly IConteudoPublicacaoRepository _conteudoRepo;
        private readonly ContextoBDNzola _contexto; // Usado apenas se precisares controlar a transação aqui

        public PublicacaoService(
            IPublicacaoRepository publicacaoRepo,
            IConteudoPublicacaoRepository conteudoRepo,
            ContextoBDNzola contexto
        )
        {
            _publicacaoRepo = publicacaoRepo;
            _conteudoRepo = conteudoRepo;
            _contexto = contexto;
        }

        public async Task<PublicacaoFeedDto?> CriarAsync(
            int utilizadorId,
            CriarPublicacaoRequestDto publicacaoDto
        )
        {
            // 1. Validação de Fluxo: Verifica se o utilizador existe de facto
            bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );
            if (!utilizadorExiste)
                return null;

            await _publicacaoRepo.ExecutarEmEstrategiaAsync(async () =>
            {
                // 2. Inicia a transação através da interface do repositório
                await _publicacaoRepo.IniciarTransacaoAsync();
                try
                {
                    // 2. Cria a entidade principal (Coração da Publicação)
                    var publicacao = publicacaoDto.ParaPublicacaoDePublicacaoDto(utilizadorId);
                    if (publicacaoDto.Conteudos != null)
                    {
                        foreach (var item in publicacaoDto.Conteudos)
                        {
                            string conteudoResolvido = string.Empty;

                            if (item.TipoConteudo == TipoConteudo.Texto)
                            {
                                conteudoResolvido = item.Texto ?? string.Empty;
                            }
                            else
                            {
                                // Faz o upload real e injeta a string do caminho
                                conteudoResolvido = await SalvarFicheiroNoServidorAsync(
                                    item.Ficheiro
                                );
                            }
                            var conteudoPublicacao =
                                conteudoPubDto.ParaConteudoPublicacaoDeItemConteudoRequestDto(
                                    publicacao.Id,
                                    conteudoResolvido
                                );
                            publicacao.Conteudos.Add(conteudoPublicacao);
                        }
                    }

                await _publicacaoRepo.AdicionarAsync(publicacao);
                    await _publicacaoRepo.SalvarAsync();

                    // Confirma a transação se tudo correr bem
                    await _publicacaoRepo.ConfirmarTransacaoAsync();

                    // 5. Devolve o DTO final estruturado para o Feed (Usa o teu mapper de saída)
                    return publicacao.ToPublicacaoFeedDto(); // Salva primeiro para gerar o PublicacaoId
                }
                catch (Exception)
                {
                    // Se algo falhar (ex: erro de disco ou BD), desfaz tudo e descarta os registos a meio
                    // Se houver erro, cancela e limpa a transação pelo repositório
                    await _publicacaoRepo.CancelarTransacaoAsync();
                    throw;
                }
            });
        }

        // Função auxiliar privada para isolar o upload físico
        private async Task<string> SalvarFicheiroNoServidorAsync(IFormFile? ficheiro)
        {
            if (ficheiro == null || ficheiro.Length == 0)
                return "/uploads/default.png";

            var nomeFicheiro = $"{Guid.NewGuid()}{Path.GetExtension(ficheiro.FileName)}";
            var caminhoPasta = Path.Combine("wwwroot", "uploads");
            Directory.CreateDirectory(caminhoPasta);

            var caminhoCompleto = Path.Combine(caminhoPasta, nomeFicheiro);
            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await ficheiro.CopyToAsync(stream);
            }

            return $"/uploads/{nomeFicheiro}";
        }
    }
}
