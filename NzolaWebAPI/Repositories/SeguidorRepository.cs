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
    public class SeguidorRepository : ISeguidorRepository
    {
        private readonly ContextoBDNzola _contexto;
        private readonly IUtilizadorRepository _utilizadorRepo;

        public SeguidorRepository(ContextoBDNzola contexto, IUtilizadorRepository utilizadorRepo)
        {
            _contexto = contexto;
            _utilizadorRepo = utilizadorRepo;
        }

        public async Task<List<Seguidor>> ListarSeguidoresPorUtilizadorAsync(int id)
        {
            return await _contexto.Seguidores.Where(s => s.SeguidoId == id).ToListAsync();
        }

        public async Task<Seguidor?> ObterPorRelacaoAsync(int seguidorId, int seguidoId)
        {
            return await _contexto.Seguidores.FirstOrDefaultAsync(b =>
                b.SeguidorId == seguidorId && b.SeguidoId == seguidoId
            );
        }

        public async Task<Seguidor> SelecionarRelacaoIdAsync(int id)
        {
            return await _contexto.Seguidores.FirstOrDefaultAsync(s => s.Id == id);
        }

        public void Remover(Seguidor seguidor)
        {
            _contexto.Seguidores.Remove(seguidor);
        }

        public async Task AdicionarAsync(Seguidor seguidor)
        {
            await _contexto.Seguidores.AddAsync(seguidor);
        }

        public async Task<bool> SalvarAsync()
        {
            return (await _contexto.SaveChangesAsync()) > 0;
        }
    }
}
