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
    public class PublicacaoRepository : IPublicacaoRepository
    {
        private readonly ContextoBDNzola _contexto;
        private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction _transacaoAtiva;

        public PublicacaoRepository(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task<bool> ExisteAsync(int id)
        {
            return await _contexto.Publicacoes.AnyAsync(p => p.Id == id);
        }

        public async Task AdicionarAsync(Publicacao publicacao)
        {
            await _contexto.Publicacoes.AddAsync(publicacao);
        }

        public async Task<bool> SalvarAsync()
        {
            return (await _contexto.SaveChangesAsync()) > 0;
        }

        public async Task<List<Publicacao>> ListarRecentesAsync() {
            return await _contexto
                .Publicacoes.Include(p => p.Utilizador)
                .Include(p => p.Conteudos)
                .OrderByDescending(p => p.DataPublicacao)
                .ToListAsync();
         }

        public async Task<Publicacao?> SelecionarAsync(int id) {
            return await _contexto
                .Publicacoes.Include(p => p.Utilizador)
                .Include(p => p.Conteudos)
                .FirstOrDefaultAsync(p => p.Id == id);
         }

         // Executa a ação dentro da ExecutionStrategy para lidar com quedas de conexão
        public async Task ExecutarEmEstrategiaAsync(Func<Task> acao)
        {
            var strategy = _contexto.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(acao);
        }

        public async Task IniciarTransacaoAsync()
        {
            _transacaoAtiva = await _contexto.Database.BeginTransactionAsync();
        }

        public async Task ConfirmarTransacaoAsync()
        {
            if (_transacaoAtiva != null)
            {
                await _transacaoAtiva.CommitAsync();
                await _transacaoAtiva.DisposeAsync();
            }
        }

        public async Task CancelarTransacaoAsync()
        {
            if (_transacaoAtiva != null)
            {
                await _transacaoAtiva.RollbackAsync();
                await _transacaoAtiva.DisposeAsync();
            }
        }
    }
}
