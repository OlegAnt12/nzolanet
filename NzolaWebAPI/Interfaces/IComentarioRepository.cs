using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IComentarioRepository
    {
        Task<Comentario?> ObterPorIdAsync(int id);
        Task <List<Comentario>> ListarPorPublicacaoIdAsync(int id);
        Task AdicionarAsync(Comentario comentario);
        void Remover(Comentario comentario);
        Task<bool> SalvarAsync();
    }
}
