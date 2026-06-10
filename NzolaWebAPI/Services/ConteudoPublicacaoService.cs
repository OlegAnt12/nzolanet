using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Services
{
    public class ConteudoPublicacaoService : IConteudoPublicacaoService
    {
        private readonly IConteudoPublicacaoRepository _repositorio;
        private readonly IPublicacaoRepository _publicacaoRepo;

        public ConteudoPublicacaoService(
            IConteudoPublicacaoRepository repositorio,
            IPublicacaoRepository publicacaoRepo
        )
        {
            _repositorio = repositorio;
            _publicacaoRepo = publicacaoRepo;
        }

        public async Task<List<ConteudoPublicacaoDto>> AdicionarListaAsync(
            List<ItemConteudoRequestDto> conteudosDtos,
            int publicacaoId
        )
        {
            int ultimaOrdem = await _repositorio.ObterUltimaOrdemPublicacaoAsync(publicacaoId);

            var novosConteudosModel = new List<ConteudoPublicacao>();
            var caminhosFicheirosCriados = new List<string>(); // Tracker de segurança para rollback físico

            try
            {
                foreach (var dto in conteudosDtos)
                {
                    ultimaOrdem++; // Incrementa para o novo bloco (4, 5, 6...)
                    string conteudoResolvido = string.Empty;

                    if (
                        dto.TipoConteudo.ToString() == "Imagem"
                        || dto.TipoConteudo.ToString() == "Video"
                    )
                    {
                        if (dto.Ficheiro == null || dto.Ficheiro.Length == 0)
                        {
                            throw new ArgumentException(
                                "Ficheiro multimédia em falta para o bloco solicitado."
                            );
                        }

                        // Processa o upload físico
                        conteudoResolvido = await SalvarFicheiroNoDiscoAsync(dto.Ficheiro);
                        caminhosFicheirosCriados.Add(conteudoResolvido); // Regista o ficheiro caso ocorra erro depois
                    }
                    else
                    {
                        conteudoResolvido = dto.Texto ?? string.Empty;
                    }

                    // Mapeia para o modelo de domínio
                    var novoConteudo = new ConteudoPublicacao
                    {
                        PublicacaoId = publicacaoId,
                        TipoConteudo = dto.TipoConteudo,
                        Ordem = ultimaOrdem, // Ordem calculada de forma segura
                        Conteudo = conteudoResolvido,
                    };

                    novosConteudosModel.Add(novoConteudo);
                }

                // 2. Grava tudo na base de dados de uma só vez
                await _repositorio.AdicionarListaAsync(novosConteudosModel);
                await _repositorio.SalvarAsync();

                // 3. Mapeia a lista gravada de volta para DTOs de saída do Feed
                var resultadoDtos = new List<ConteudoPublicacaoDto>();
                foreach (var model in novosConteudosModel)
                {
                    resultadoDtos.Add(model.ToConteudoPublicacaoDto()); // Usa o teu extension mapper existente
                }

                return resultadoDtos;
            }
            catch (Exception)
            {
                // FALLBACK DE INFRAESTRUTURA: Se a base de dados falhar, apaga os ficheiros do disco
                // para evitar lixo no teu servidor wwwroot/uploads
                foreach (var caminhoRelativo in caminhosFicheirosCriados)
                {
                    var caminhoCompleto = Path.Combine("wwwroot", caminhoRelativo.TrimStart('/'));
                    if (File.Exists(caminhoCompleto))
                        File.Delete(caminhoCompleto);
                }
                throw; // Repassa o erro para o Controller tratar
            }
        }

        // Função auxiliar isolada para gravação física de ficheiros
        private async Task<string> SalvarFicheiroNoDiscoAsync(IFormFile ficheiro)
        {
            var nomeFicheiro = $"{Guid.NewGuid()}{Path.GetExtension(ficheiro.FileName)}";
            var pastaUploads = Path.Combine("wwwroot", "uploads");

            if (!Directory.Exists(pastaUploads))
            {
                Directory.CreateDirectory(pastaUploads);
            }

            var caminhoCompleto = Path.Combine(pastaUploads, nomeFicheiro);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await ficheiro.CopyToAsync(stream);
            }

            return $"/uploads/{nomeFicheiro}";
        }
    }
}
