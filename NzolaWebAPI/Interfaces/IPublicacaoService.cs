using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Publicacao;

namespace NzolaWebAPI.Interfaces
{
    public interface IPublicacaoService
    {
        Task<PublicacaoFeedDto?> CriarAsync(int utilizadorId, CriarPublicacaoRequestDto publicacaoDto);
    }
}