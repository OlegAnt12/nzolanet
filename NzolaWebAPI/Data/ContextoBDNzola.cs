using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Data
{
    public class ContextoBDNzola : DbContext
    {
        public ContextoBDNzola(DbContextOptions options)
            : base(options) { }

        public DbSet<Publicacao> Publicacoes { get; set; }
        public DbSet<ConteudoPublicacao> ConteudosPublicacao { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Baze> Bazes { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Seguidor> Seguidores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CRUCIAL: Cria uma chave composta na tabela Baze
            // Isto impede que o mesmo UtilizadorId dê mais do que um Baze no mesmo PublicacaoId

            modelBuilder
                .Entity<Comentario>()
                .HasOne(c => c.Publicacao)
                .WithMany(p => p.Comentarios)
                .HasForeignKey(c => c.PublicacaoId)
                .OnDelete(DeleteBehavior.Restrict); // Desativa cascade da Publicação para o Comentário

            // 2. CORREÇÃO DO NOVO PROBLEMA: Resolve o ciclo das Bazes
            modelBuilder
                .Entity<Baze>()
                .HasOne(b => b.Publicacao)
                .WithMany(p => p.Bazes)
                .HasForeignKey(b => b.PublicacaoId)
                .OnDelete(DeleteBehavior.Restrict); // Desativa cascade da Publicação para a Baze


// 🔥 SOLUÇÃO: Quebrar o ciclo de cascade path entre Publicação e Utilizador
    modelBuilder.Entity<Publicacao>()
        .HasOne(p => p.Utilizador)          // Uma Publicação tem um Utilizador
        .WithMany(u => u.Publicacoes)       // Um Utilizador tem muitas Publicações
        .HasForeignKey(p => p.AutorId)      // A chave estrangeira é o AutorId
        .OnDelete(DeleteBehavior.Restrict);

    // Se o erro persistir noutras tabelas como Comentarios ou Bazes, 
    // podes aplicar a mesma lógica nelas da seguinte forma:
    modelBuilder.Entity<Baze>()
        .HasOne(b => b.Utilizador)
        .WithMany()
        .HasForeignKey(b => b.UtilizadorId)
        .OnDelete(DeleteBehavior.Restrict);

            // 3. Define a Chave Composta da Baze (Garante 1 baze por post por utilizador)
            modelBuilder.Entity<Baze>().HasKey(b => new { b.UtilizadorId, b.PublicacaoId });

            // 🔗 Configuração da Tabela de Seguidores (Fluent API)
            /*modelBuilder.Entity<Seguidor>(entity =>
            {
                // 1. Define a chave primária da tabela pivot
                entity.HasKey(s => s.Id);

                // 2. Mapeia a relação de quem está a SEGUIR (O Seguidor)
                entity
                    .HasOne(s => s.Utilizador)
                    .WithMany(u => u.Seguindo) // Garante que tens 'public ICollection<Seguidor> Seguindo { get; set; }' no Utilizador.cs
                    .HasForeignKey(s => s.SeguidorId)
                    .OnDelete(DeleteBehavior.Restrict); // 🔥 OBRIGATÓRIO: Desativa a cascata para evitar ciclos no SQL Server

                // 3. Mapeia a relação de quem está a SER SEGUIDO (O Alvo/Destino)
                entity
                    .HasOne(s => s.UtilizadorSeguido)
                    .WithMany(u => u.Seguidores) // Garante que tens 'public ICollection<Seguidor> Seguidores { get; set; }' no Utilizador.cs
                    .HasForeignKey(s => s.SeguidoId)
                    .OnDelete(DeleteBehavior.Restrict); // 🔥 OBRIGATÓRIO: Desativa a cascata aqui também
            });*/
        }
    }
}
