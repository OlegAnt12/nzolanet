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
    public class ComentarioRepository : IComentarioRepository
    {
        private readonly ContextoBDNzola _contexto;

        public ComentarioRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task<Comentario?> ObterPorIdAsync(int id)
        {
            return await _contexto.Comentarios.FindAsync(id);
        }

        public async Task AdicionarAsync(Comentario comentario)
        {
            await _contexto.Comentarios.AddAsync(comentario);
        }

        public void Remover(Comentario comentario)
        {
            _contexto.Comentarios.Remove(comentario);
        }

        public async Task<bool> SalvarAsync()
        {
            return await _contexto.SaveChangesAsync() > 0;
        }
    }
}
