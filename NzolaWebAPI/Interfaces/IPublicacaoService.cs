using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IPublicacaoService
    {
        Task<PublicacaoFeedDto?> CriarAsync(
            int utilizadorId,
            CriarPublicacaoRequestDto publicacaoDto,
            string? textoFallback = null,
            Microsoft.AspNetCore.Http.IFormFile? file = null
        );
        Task<Publicacao?> ActualizarAsync(int publicacaoId, ActualizarPublicacaoRequestDto putDto);
        Task<Publicacao?> EliminarAsync(int publicacaoId);
        Task<(List<PublicacaoFeedDto> Publicacoes, int Total)> ListarFeedAsync(int? utilizadorLogadoId = null, int pagina = 1, int tamanho = 10);
        Task<List<PublicacaoFeedDto>> PesquisarPublicacoesAsync(string termo);
    }
}
