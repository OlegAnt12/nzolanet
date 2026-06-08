using NzolaWebAPI.Models;
using NzolaWebAPI.DTOs.Utilizador;


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
                Email = utilizador.Email,
                Biografia = utilizador.Biografia,
                Privacidade = utilizador.Privacidade,
                EstadoConta = utilizador.EstadoConta
            };

        }

        public static Utilizador ToUtilizadorFromCriarDto(this CriarUtilizadorRequestDto utilizadorDto)
        {
            return new Utilizador
            {
                NomeCompleto = utilizadorDto.NomeCompleto,
                Email = utilizadorDto.Email,
                FotoPerfil = utilizadorDto.FotoPerfil,
                PalavraPasse = utilizadorDto.PalavraPasse,
                genero = utilizadorDto.genero,
                DataNascimento = utilizadorDto.DataNascimento
            };
        }
    }
}