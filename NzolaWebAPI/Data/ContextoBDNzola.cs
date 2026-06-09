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

            modelBuilder
                .Entity<Publicacao>()
                .HasOne(p => p.Utilizador) // Uma Publicação tem um Autor/Utilizador
                .WithMany(u => u.Publicacoes) // Um Utilizador tem muitas Publicações
                .HasForeignKey(p => p.AutorId) // A chave estrangeira na tabela Publicação é AutorId
                .OnDelete(DeleteBehavior.Cascade); // Se o Utilizador morrer, os posts morrem com ele! (PERMITIDO)

            // 3. Define a Chave Composta da Baze (Garante 1 baze por post por utilizador)
            modelBuilder.Entity<Baze>().HasKey(b => new { b.UtilizadorId, b.PublicacaoId });

            // NOTA: Se a tabela 'Seguidor' também der erro de ciclo por relacionar Utilizador duas vezes
            // (SeguidoId e SeguidorId), adiciona também:
            // modelBuilder.Entity<Seguidor>()
            //     .HasOne(s => s.SeguidorUtilizador)
            //     .WithMany()
         
           //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
