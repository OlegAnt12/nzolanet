using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IConteudoPublicacaoService
    {
        Task<List<ConteudoPublicacaoDto>> AdicionarListaAsync(
            List<ItemConteudoRequestDto> dtos,
            int publicacaoId
        );
    }
}
