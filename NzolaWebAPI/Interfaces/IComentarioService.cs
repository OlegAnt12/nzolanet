using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NzolaWebAPI.DTOs.Comentario;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Interfaces
{
    public interface IComentarioService
    {
        Task<ComentarioResultadoDto> AdicionarAsync(
            int publicacaoId,
            int utilizadorId,
            AdicionarComentarioRequestDto dto
        );
        Task<ComentarioResultadoDto> EditarAsync(int id, EditarComentarioRequestDto dto);
        Task<List<ComentarioDto>> ListarAsync(int id);
        Task<ComentarioResultadoDto> ExcluirAsync(int id);
    }
}
