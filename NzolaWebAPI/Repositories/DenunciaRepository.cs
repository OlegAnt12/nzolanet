using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Repositories
{
    public class DenunciaRepository : IDenunciaRepository
    {
        private readonly ContextoBDNzola _contexto;

        public DenunciaRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task AdicionarAsync(Denuncia denuncia)
        {
            await _contexto.Denuncias.AddAsync(denuncia);
        }

        public async Task<List<Denuncia>> ListarTodasAsync()
        {
            return await _contexto.Denuncias
                .Include(d => d.Denunciante)
                .OrderByDescending(d => d.DataDenuncia)
                .ToListAsync();
        }

        public async Task<List<Denuncia>> ListarPorEntidadeAsync(TipoEntidade tipoEntidade, int idEntidade)
        {
            return await _contexto.Denuncias
                .Include(d => d.Denunciante)
                .Where(d => d.TipoEntidade == tipoEntidade && d.IdEntidade == idEntidade)
                .OrderByDescending(d => d.DataDenuncia)
                .ToListAsync();
        }

        public async Task<bool> SalvarAsync()
        {
            return await _contexto.SaveChangesAsync() > 0;
        }

        public async Task<int> ContarTodasAsync()
        {
            return await _contexto.Denuncias.CountAsync();
        }

        public async Task<int> ContarPendentesAsync()
        {
            return await _contexto.Denuncias.CountAsync(d => d.EstadoDenuncia == EstadoDenuncia.Pendente);
        }
    }
}
