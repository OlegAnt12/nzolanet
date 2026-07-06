using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Repositories
{
    public class UtilizadorRepository : IUtilizadorRepository
    {
        private readonly ContextoBDNzola _context;

        public UtilizadorRepository(ContextoBDNzola context)
        {
            _context = context;
        }

        public async Task<Utilizador?> ObterPorEmailAsync(string email)
        {
            return await _context.Utilizadores.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<Utilizador?> ObterPorIdAsync(int id)
        {
            return await _context.Utilizadores.FindAsync(id);
        }

        public async Task<Utilizador?> ObterPorTokenRedefinirPasswordAsync(string token)
        {
            return await _context.Utilizadores.FirstOrDefaultAsync(
                u => u.ResetTokenRedefinirPassword == token
            );
        }

        public async Task AdicionarAsync(Utilizador utilizador)
        {
            await _context.Utilizadores.AddAsync(utilizador);
        }

        public async Task<bool> ExisteEmailAsync(string email)
        {
            return await _context.Utilizadores.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> SalvarAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> ContarTodosAsync()
        {
            return await _context.Utilizadores.CountAsync();
        }
    }
}
