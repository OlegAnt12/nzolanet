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
    public class BazesRepository : IBazesRepository
    {
        private readonly ContextoBDNzola _contexto;

        public BazesRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Baze>> GetBazesPorPublicacaoAsync(int id)
        {
            return await _contexto.Bazes.Where(b => b.PublicacaoId == id).ToListAsync();
        }
    }
}
