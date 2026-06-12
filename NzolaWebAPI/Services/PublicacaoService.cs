using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Services
{
    public class PublicacaoService : IPublicacaoService
    {
        private readonly IPublicacaoRepository _publicacaoRepo;
        private readonly ContextoBDNzola _contexto; // Usado apenas se precisares controlar a transação aqui

        public PublicacaoService(
            IPublicacaoRepository publicacaoRepo,
            ContextoBDNzola contexto
        )
        {
            _publicacaoRepo = publicacaoRepo;
            _contexto = contexto;
        }

        public async Task<PublicacaoFeedDto?> CriarAsync(
            int utilizadorId,
            CriarPublicacaoRequestDto publicacaoDto,
            string? textoFallback = null,
            Microsoft.AspNetCore.Http.IFormFile? file = null
        )
        {
            bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );
            if (!utilizadorExiste)
                return null;

            Publicacao? publicacao = null;

            await _publicacaoRepo.ExecutarEmEstrategiaAsync(async () =>
            {
                await _publicacaoRepo.IniciarTransacaoAsync();
                try
                {
                    // Normalização e validações agora ocorrem nesta camada (Service)
                    if (publicacaoDto == null)
                    {
                        publicacaoDto = new CriarPublicacaoRequestDto();
                    }

                    // Preenche Texto a partir do fallback enviado pelo controller, se necessário
                    if (string.IsNullOrWhiteSpace(publicacaoDto.Texto) && !string.IsNullOrWhiteSpace(textoFallback))
                    {
                        publicacaoDto.Texto = textoFallback;
                    }

                    // Se a lista de ficheiros vier vazia e foi passado um ficheiro isolado, injeta-o
                    if ((publicacaoDto.Ficheiros == null || !publicacaoDto.Ficheiros.Any()) && file != null)
                    {
                        publicacaoDto.Ficheiros = new List<Microsoft.AspNetCore.Http.IFormFile> { file };
                    }

                    if (string.IsNullOrWhiteSpace(publicacaoDto.Texto) && (publicacaoDto.Ficheiros == null || !publicacaoDto.Ficheiros.Any()))
                    {
                        throw new ArgumentException("A publicação necessita de um conteúdo válido (texto ou pelo menos um ficheiro).");
                    }

                    publicacao = publicacaoDto.ParaPublicacaoDePublicacaoDto(utilizadorId);

                    var ficheirosResolvidos = new List<FicheiroConteudo>();

                    if (publicacaoDto.Ficheiros != null && publicacaoDto.Ficheiros.Any())
                    {
                        foreach (var ficheiro in publicacaoDto.Ficheiros)
                        {
                            if (ficheiro == null || ficheiro.Length == 0)
                                continue;

                            var caminhoSalvo = await SalvarFicheiroNoServidorAsync(ficheiro);
                            ficheirosResolvidos.Add(new FicheiroConteudo
                            {
                                Publicacao = publicacao,
                                CaminhoFicheiro = caminhoSalvo,
                                TipoMime = ficheiro.ContentType,
                                TamanhoBytes = ficheiro.Length
                            });
                        }
                    }

                    publicacao.Ficheiros = ficheirosResolvidos;

                    await _publicacaoRepo.AdicionarAsync(publicacao);
                    await _publicacaoRepo.SalvarAsync();
                    await _publicacaoRepo.ConfirmarTransacaoAsync();
                }
                catch (Exception)
                {
                    await _publicacaoRepo.CancelarTransacaoAsync();
                    throw;
                }
            });

            return publicacao != null ? publicacao.ToPublicacaoFeedDto() : null;
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
