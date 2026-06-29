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
                FotoPerfil =
                    utilizador.FotoPerfil != null
                        ? Convert.ToBase64String(utilizador.FotoPerfil)
                        : null,
                Privacidade = utilizador.Privacidade,
                Genero = utilizador.Genero,
                DataNascimento = utilizador.DataNascimento,
                ConcordaComTermos = utilizador.ConcordaComTermos,
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
                ConcordaComTermos = utilizadorDto.ConcordaComTermos,
            };
        }

        public static AutorPublicacaoDto ToAutorPublicacaoDto(this Utilizador modelUtilizador)
        {
            return new AutorPublicacaoDto
            {
                Id = modelUtilizador.Id,
                NomeCompleto = modelUtilizador.NomeCompleto,
                NomeUtilizador = modelUtilizador.NomeCompleto,
                FotoPerfil =
                    modelUtilizador.FotoPerfil != null
                        ? Convert.ToBase64String(modelUtilizador.FotoPerfil)
                        : null,
                Seguidores = modelUtilizador.Seguidores.Select(s => s.ToSeguidorFeedDto()).ToList(),
                Seguidos = modelUtilizador.Seguindo.Select(s => s.ToSeguidorFeedDto()).ToList()
            };
        }

        public static UtilizadorSimplificadoDto ToUtilizadorSimplificadoDto(this Utilizador modelUtilizador)
        {
            return new UtilizadorSimplificadoDto
            {
                Id = modelUtilizador.Id,
                NomeCompleto = modelUtilizador.NomeCompleto,
                NomeUtilizador = modelUtilizador.NomeUtilizador,
                FotoPerfil =
                    modelUtilizador.FotoPerfil != null
                        ? Convert.ToBase64String(modelUtilizador.FotoPerfil)
                        : null,
                
            };
        }
    }
}
