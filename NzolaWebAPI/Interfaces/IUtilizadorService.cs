using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IUtilizadorService
    {
        Task<string?> LoginAsync(string email, string palavraPasse);

        
        Task<bool> RegistarAsync(Utilizador utilizador, string palavraPasse);
    }
}