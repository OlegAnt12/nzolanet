using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

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

        public async Task<List<Publicacao>> ListarRecentesPorFeedAsync(int? utilizadorLogadoId = null)
        {
            var query = _contexto
                .Publicacoes.Where(p => p.Existencia != (EstadoExistenciaLogica)0)
                .Include(p => p.Utilizador).ThenInclude(u => u.Seguidores)
                .Include(p => p.Utilizador).ThenInclude(u => u.Seguindo)
                .Include(p => p.Ficheiros)
                .Include(p => p.Comentarios).ThenInclude(c => c.Utilizador)
                .AsQueryable();

            if (utilizadorLogadoId.HasValue)
            {
                var seguindoIds = await _contexto.Seguidores
                    .Where(s => s.SeguidorId == utilizadorLogadoId.Value)
                    .Select(s => s.SeguidoId)
                    .ToListAsync();

                query = query.Where(p =>
                    p.Utilizador.Privacidade == Models.Enums.EstadoAcesso.Publico ||
                    seguindoIds.Contains(p.AutorId));
            }
            else
            {
                query = query.Where(p => p.Utilizador.Privacidade == Models.Enums.EstadoAcesso.Publico);
            }

            return await query
                .OrderByDescending(p => p.DataPublicacao)
                .ToListAsync();
        }

        public async Task<List<Publicacao>> ListarRecentesAsync()
        {
            return await _contexto
                .Publicacoes.Where(p => p.Existencia != (EstadoExistenciaLogica)0)
                .Include(p => p.Utilizador)
                .Include(p => p.Ficheiros)
                .Include(p => p.Comentarios)
                .OrderByDescending(p => p.DataPublicacao)
                .ToListAsync();
        }

        public async Task<Publicacao?> SelecionarAsync(int id)
        {
            return await _contexto
                .Publicacoes.Where(p => p.Existencia != 0)
                .Include(p => p.Utilizador)
                .Include(p => p.Ficheiros)
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

        public async Task<int> ContarPorUtilizadorAsync(int utilizadorId)
        {
            return await _contexto.Publicacoes.Where(p => p.AutorId == utilizadorId && p.Existencia != 0).CountAsync();
        }
    }
}
