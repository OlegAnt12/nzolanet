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
        public ContextoBDNzola() { }

        public ContextoBDNzola(DbContextOptions<ContextoBDNzola> options) : base(options)
        {
            
        }

        public DbSet<Publicacao> Publicacoes { get; set; }
        public DbSet<ConteudoPublicacao> ConteudosPublicacao { get; set; }
        public DbSet<Comentario> Comentarios { get; set; }
        public DbSet<Baze> Bazes { get; set; }
        public DbSet<Notificacao> Notificacoes { get; set; }
        public DbSet<Utilizador> Utilizadores{get; set;}    
    }
}