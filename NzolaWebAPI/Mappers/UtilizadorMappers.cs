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
    }
}