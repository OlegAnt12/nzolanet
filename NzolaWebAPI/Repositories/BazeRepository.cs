using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Repositories
{
    public class BazeRepository : IBazeRepository
    {
        private readonly ContextoBDNzola _contexto;

        public BazeRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Baze>> GetBazesPorPublicacaoAsync(int id)
        {
            return await _contexto.Bazes.Where(b => b.PublicacaoId == id).ToListAsync();
        }

        public async Task<Baze> SelecionarBazeAsync(int id)
        {
            return await _contexto.Bazes.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Baze?> ObterPorPublicacaoEUtilizadorAsync(
            int publicacaoId,
            int utilizadorId
        )
        {
            return await _contexto.Bazes.FirstOrDefaultAsync(b =>
                b.PublicacaoId == publicacaoId && b.UtilizadorId == utilizadorId
            );
        }

        public async Task AdicionarAsync(Baze baze)
        {
            await _contexto.Bazes.AddAsync(baze);
        }

        public void Remover(Baze baze)
        {
            _contexto.Bazes.Remove(baze);
        }

        public async Task<bool> SalvarAsync()
        {
            return await _contexto.SaveChangesAsync() > 0;
        }
    }
}
