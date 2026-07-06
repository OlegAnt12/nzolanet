using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.Dtos.Admin;
using NzolaWebAPI.DTOs.Denuncia;
using NzolaWebAPI.DTOs.Publicacao;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly ContextoBDNzola _contexto;

        public AdminService(ContextoBDNzola contexto)
        {
            _contexto = contexto;
        }

        public async Task<AdminDashboardDto> ObterDashboardAsync()
        {
            var totalUtilizadores = await _contexto.Utilizadores.CountAsync();
            var totalPublicacoes = await _contexto.Publicacoes.CountAsync(p => p.Existencia != 0);
            var totalBazes = await _contexto.Bazes.CountAsync();
            var totalDenuncias = await _contexto.Denuncias.CountAsync();
            var denunciasPendentes = await _contexto.Denuncias.CountAsync(d => d.EstadoDenuncia == EstadoDenuncia.Pendente);
            var utilizadoresAtivos = await _contexto.Utilizadores.CountAsync(u => u.EstadoConta == EstadoConta.Activa);
            var utilizadoresPrivados = await _contexto.Utilizadores.CountAsync(u => u.Privacidade == EstadoAcesso.Privado);

            return new AdminDashboardDto
            {
                TotalUtilizadores = totalUtilizadores,
                TotalPublicacoes = totalPublicacoes,
                TotalBazes = totalBazes,
                TotalDenuncias = totalDenuncias,
                DenunciasPendentes = denunciasPendentes,
                UtilizadoresAtivos = utilizadoresAtivos,
                UtilizadoresPrivados = utilizadoresPrivados
            };
        }

        public async Task<List<UtilizadorDto>> ListarUtilizadoresAsync()
        {
            return await _contexto
                .Utilizadores
                .Select(u => u.ToUtilizadorDto())
                .ToListAsync();
        }

        public async Task<List<PublicacaoFeedDto>> ListarPublicacoesAsync()
        {
            var publicacoes = await _contexto
                .Publicacoes
                .Where(p => p.Existencia != 0)
                .Include(p => p.Utilizador)
                .Include(p => p.Ficheiros)
                .OrderByDescending(p => p.DataPublicacao)
                .ToListAsync();

            return publicacoes.Select(p => p.ToPublicacaoFeedDto()).ToList();
        }

        public async Task<UtilizadorDto?> CriarUtilizadorAsync(CriarUtilizadorAdminRequestDto dto)
        {
            var emailExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Email.ToLower() == dto.Email.ToLower()
            );

            if (emailExiste) return null;

            var nomeUtilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.NomeUtilizador.ToLower() == dto.NomeUtilizador.ToLower()
            );

            if (nomeUtilizadorExiste) return null;

            var utilizador = new Utilizador
            {
                NomeCompleto = dto.NomeCompleto,
                NomeUtilizador = dto.NomeUtilizador,
                Email = dto.Email,
                PalavraPasse = dto.PalavraPasse,
                Genero = dto.Genero,
                DataNascimento = dto.DataNascimento,
                NivelAcesso = NivelAcesso.Admin,
                DataRegistro = DateTime.UtcNow,
                ConcordaComTermos = true,
                Privacidade = EstadoAcesso.Publico,
                EstadoConta = EstadoConta.Activa,
            };

            _contexto.Utilizadores.Add(utilizador);
            await _contexto.SaveChangesAsync();

            return utilizador.ToUtilizadorDto();
        }

        public async Task<List<DenunciaDto>> ListarDenunciasAsync()
        {
            return await _contexto
                .Denuncias
                .Include(d => d.Denunciante)
                .OrderByDescending(d => d.DataDenuncia)
                .Select(d => new DenunciaDto
                {
                    Id = d.Id,
                    TipoEntidade = d.TipoEntidade,
                    IdEntidade = d.IdEntidade,
                    Motivo = d.Motivo,
                    Descricao = d.Descricao,
                    DenuncianteId = d.DenuncianteId,
                    NomeDenunciante = d.Denunciante.NomeCompleto,
                    DataDenuncia = d.DataDenuncia,
                    EstadoDenuncia = d.EstadoDenuncia,
                })
                .ToListAsync();
        }
    }
}
