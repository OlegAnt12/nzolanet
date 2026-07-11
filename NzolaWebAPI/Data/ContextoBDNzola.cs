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
        public DbSet<FicheiroConteudo> FicheirosConteudo { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Baze> Bazes { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }
        public DbSet<Seguidor> Seguidores { get; set; }
        public DbSet<Denuncia> Denuncias { get; set; }
        public DbSet<PedidoSeguir> PedidosSeguir { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

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
            modelBuilder
                .Entity<Publicacao>()
                .HasOne(p => p.Utilizador) // Uma Publicação tem um Utilizador
                .WithMany(u => u.Publicacoes) // Um Utilizador tem muitas Publicações
                .HasForeignKey(p => p.AutorId) // A chave estrangeira é o AutorId
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Baze>()
                .HasOne(b => b.Utilizador)
                .WithMany(u => u.Bazes)
                .HasForeignKey(b => b.UtilizadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 3. Define a Chave Composta da Baze (Garante 1 baze por post por utilizador)
            modelBuilder.Entity<Baze>().HasKey(b => new { b.UtilizadorId, b.PublicacaoId });

            // 🔗 Relacionamento 1:N entre Publicacao e FicheiroConteudo
            modelBuilder
                .Entity<Publicacao>()
                .HasMany(p => p.Ficheiros)
                .WithOne(f => f.Publicacao)
                .HasForeignKey(f => f.PublicacaoId)
                .OnDelete(DeleteBehavior.Cascade); // Se apagar publicação, apaga os ficheiros

            // 🔗 Configuração da Tabela de Seguidores (Fluent API)
            modelBuilder.Entity<Seguidor>(entity =>
            {
                // 1. Define a chave primária da tabela pivot
                entity.HasKey(s => s.Id);

                // 2. Mapeia a relação de quem está a SEGUIR (O Seguidor)
                entity
                    .HasOne(s => s.UtilizadorSeguidor)
                    .WithMany(u => u.Seguindo) // Garante que tens 'public ICollection<Seguidor> Seguindo { get; set; }' no Utilizador.cs
                    .HasForeignKey(s => s.SeguidorId)
                    .OnDelete(DeleteBehavior.Restrict); // 🔥 OBRIGATÓRIO: Desativa a cascata para evitar ciclos no SQL Server

                // 3. Mapeia a relação de quem está a SER SEGUIDO (O Alvo/Destino)
                entity
                    .HasOne(s => s.UtilizadorSeguido)
                    .WithMany(u => u.Seguidores) // Garante que tens 'public ICollection<Seguidor> Seguidores { get; set; }' no Utilizador.cs
                    .HasForeignKey(s => s.SeguidoId)
                    .OnDelete(DeleteBehavior.Restrict); // 🔥 OBRIGATÓRIO: Desativa a cascata aqui também
            });

            // Fluent API: Assegura que o Email é único (redundante com o atributo Index, mas explícito aqui)
            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();


                // Armazena o enum Genero como string e define o tipo de coluna
                entity.Property(u => u.Genero)
                    .HasConversion<string>()
                    .HasColumnType("nvarchar(10)");

                entity.HasIndex(u => u.NomeUtilizador).IsUnique();

                entity.Property(u => u.NomeUtilizador)
                    .HasMaxLength(50)
                    .IsRequired();

                // Adiciona constraint para aceitar apenas Masculino e Feminino
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Utilizadores_Genero",
                    "Genero IN ('Masculino','Feminino')"
                ));
            });

            modelBuilder.Entity<Denuncia>(entity =>
            {
                entity
                    .HasOne(d => d.Denunciante)
                    .WithMany()
                    .HasForeignKey(d => d.DenuncianteId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PedidoSeguir>(entity =>
            {
                entity
                    .HasOne(p => p.UtilizadorSeguidor)
                    .WithMany()
                    .HasForeignKey(p => p.SeguidorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity
                    .HasOne(p => p.UtilizadorSeguido)
                    .WithMany()
                    .HasForeignKey(p => p.SeguidoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notificacao>(entity =>
           {


               // Armazena o enum Genero como string e define o tipo de coluna
               entity.Property(n => n.Tipo)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar(10)");

               entity.Property(n => n.Mensagem)
                   .HasMaxLength(256)
                   .IsRequired();

               entity
                .HasOne(n => n.UtilizadorNotificacao)
                .WithMany(u => u.Notificacoes)
                .HasForeignKey(n => n.UtilizadorId)
                .OnDelete(DeleteBehavior.Restrict);

               // Adiciona constraint para aceitar apenas Masculino e Feminino
               entity.ToTable(t => t.HasCheckConstraint(
                   "CK_Notificacoes_Tipo",
                   "Tipo IN ('Baze','Comentario', 'Seguidor')"
               ));
           });
        }
    }
}
