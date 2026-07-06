using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IUtilizadorService
    {
        Task<string?> LoginAsync(string email, string palavraPasse);

        Task<bool> RegistarAsync(Utilizador utilizador, string palavraPasse);

        Task<Utilizador?> AtualizarPerfilAsync(int utilizadorId, ActualizarPerfilRequestDto dto);
        Task<UtilizadorDto?> ObterPorIdServiceAsync(int id, int? utilizadorLogadoId = null);
        Task<object> ObterEstatisticasAsync(int id);

        Task<string?> GerarTokenRedefinirPasswordAsync(string email);
        Task<bool> RedefinirPasswordAsync(string token, string novaPalavraPasse);
    }
}
