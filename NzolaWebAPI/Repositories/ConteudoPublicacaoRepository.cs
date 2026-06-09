using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.ConteudoPublicacao;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Repositories
{
    public class ConteudoPublicacaoRepository : IConteudoPublicacaoRepository
    {
        private readonly ContextoBDNzola _contexto;

        public ConteudoPublicacaoRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task AdicionarAsync(List<ConteudoPublicacao> conteudos)
        {
            await _contexto.ConteudosPublicacao.AddRangeAsync(conteudos);
        }

        public async Task AdicionarListaAsync(List<ConteudoPublicacao> conteudos)
        {
            await _contexto.ConteudosPublicacao.AddRangeAsync(conteudos);
        }

        public async Task<int> ObterUltimaOrdemPublicacaoAsync(int publicacaoId)
        {
            return await _contexto
                    .ConteudosPublicacao.Where(c => c.PublicacaoId == publicacaoId)
                    .Select(c => (int?)c.Ordem)
                    .MaxAsync()
                ?? 0;
        }

        public async Task<bool> SalvarAsync()
        {
            return (await _contexto.SaveChangesAsync()) > 0;
        }
    }
}
