using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Models;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(ContextoBDNzola contexto)
        {
            if (await contexto.Utilizadores.AnyAsync())
                return;

            var admin = new Utilizador
            {
                NomeUtilizador = "admin",
                NomeCompleto = "Administrador",
                Email = "admin@nzolanet.com",
                PalavraPasse = "admin123",
                Genero = Genero.Masculino,
                NivelAcesso = NivelAcesso.Admin,
                Privacidade = EstadoAcesso.Publico,
                EstadoConta = EstadoConta.Activa,
                DataNascimento = new DateTime(1990, 1, 1),
                ConcordaComTermos = true,
            };

            var joao = new Utilizador
            {
                NomeUtilizador = "joao",
                NomeCompleto = "João Silva",
                Email = "joao@nzolanet.com",
                PalavraPasse = "joao123",
                Genero = Genero.Masculino,
                NivelAcesso = NivelAcesso.Normal,
                Privacidade = EstadoAcesso.Publico,
                EstadoConta = EstadoConta.Activa,
                DataNascimento = new DateTime(1995, 5, 15),
                ConcordaComTermos = true,
            };

            var maria = new Utilizador
            {
                NomeUtilizador = "maria",
                NomeCompleto = "Maria Santos",
                Email = "maria@nzolanet.com",
                PalavraPasse = "maria123",
                Genero = Genero.Feminino,
                NivelAcesso = NivelAcesso.Normal,
                Privacidade = EstadoAcesso.Publico,
                EstadoConta = EstadoConta.Activa,
                DataNascimento = new DateTime(1998, 8, 20),
                ConcordaComTermos = true,
            };

            await contexto.Utilizadores.AddRangeAsync(admin, joao, maria);
            await contexto.SaveChangesAsync();

            var publicacaoJoao = new Publicacao
            {
                AutorId = joao.Id,
                Texto = "Olá mundo! Esta é a minha primeira publicação na NzolaNet!",
                DataPublicacao = DateTime.Now.AddDays(-2),
            };

            var publicacaoMaria = new Publicacao
            {
                AutorId = maria.Id,
                Texto = "Bem-vindos à NzolaNet! Vamos construir algo incrível juntos!",
                DataPublicacao = DateTime.Now.AddDays(-1),
            };

            await contexto.Publicacoes.AddRangeAsync(publicacaoJoao, publicacaoMaria);
            await contexto.SaveChangesAsync();

            var seguir = new Seguidor
            {
                SeguidorId = joao.Id,
                SeguidoId = maria.Id,
            };

            await contexto.Seguidores.AddAsync(seguir);
            await contexto.SaveChangesAsync();
        }
    }
}
