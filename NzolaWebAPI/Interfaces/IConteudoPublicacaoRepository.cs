using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IConteudoPublicacaoRepository
    {
        Task AdicionarListaAsync(List<ConteudoPublicacao> conteudos);
        Task<int> ObterUltimaOrdemPublicacaoAsync(int publicacaoId);
        Task<bool> SalvarAsync();
    }
}
