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

        public Task<List<Baze>> GetBazesPorPublicacaoAsync()
        {
            return _contexto
                .Bazes.Where(b => b.PublicacaoId == id)
                .Select(b => b.ToBazeDto())
                .ToListAsync();
        }
    }
}
