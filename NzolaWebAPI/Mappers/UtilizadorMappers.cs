using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Mappers
{
    public static class UtilizadorMapper
    {
        public static UtilizadorDto ToUtilizadorDto(this Utilizador utilizador)
        {
            return new UtilizadorDto
            {
                Id = utilizador.Id,
                NomeCompleto = utilizador.NomeCompleto,
                NomeUtilizador = utilizador.NomeUtilizador,
                Email = utilizador.Email,
                Biografia = utilizador.Biografia,
                Privacidade = utilizador.Privacidade,
                EstadoConta = utilizador.EstadoConta,
                Genero = utilizador.Genero,
            };
        }

        public static Utilizador ToUtilizadorFromCriarDto(
            this CriarUtilizadorRequestDto utilizadorDto
        )
        {
            return new Utilizador
            {
                NomeCompleto = utilizadorDto.NomeCompleto,
                Email = utilizadorDto.Email,
                FotoPerfil = utilizadorDto.FotoPerfil != null ? utilizadorDto.FotoPerfil : null,
                PalavraPasse = utilizadorDto.PalavraPasse,
                Genero = utilizadorDto.genero,
                NomeUtilizador = utilizadorDto.NomeUtilizador,
                DataNascimento = utilizadorDto.DataNascimento,
            };
        }

        public static AutorPublicacaoDto ToAutorPublicacaoDto(this Utilizador modelUtilizador)
        {
            return new AutorPublicacaoDto
            {
                Id = modelUtilizador.Id,
                NomeCompleto = modelUtilizador.NomeCompleto,
                FotoPerfil = modelUtilizador.FotoPerfil,
            };
        }
    }
}
