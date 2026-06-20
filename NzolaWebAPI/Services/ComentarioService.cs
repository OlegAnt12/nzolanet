using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Data;
using NzolaWebAPI.DTOs.Comentario;
using NzolaWebAPI.DTOs.Utilizador;
using NzolaWebAPI.Interfaces;
using NzolaWebAPI.Mappers;
using NzolaWebAPI.Models;

namespace NzolaWebAPI.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarioRepo;
        private readonly IPublicacaoRepository _publicacaoRepo;
        private readonly ContextoBDNzola _contexto;

        //private readonly IUtilizadorRepository _utilizadorRepo;

        public ComentarioService(
            IComentarioRepository comentarioRepo,
            IPublicacaoRepository publicacaoRepo,
            ContextoBDNzola contexto
        //IUtilizadorRepository utilizadorRepo
        )
        {
            _comentarioRepo = comentarioRepo;
            _publicacaoRepo = publicacaoRepo;
            _contexto = contexto;
            //_utilizadorRepo = utilizadorRepo;
        }

        public async Task<ComentarioResultadoDto> AdicionarAsync(
            int publicacaoId,
            int utilizadorId,
            AdicionarComentarioRequestDto dto
        )
        {
            var resultado = new ComentarioResultadoDto();
            /*bool utilizadorExiste = await _contexto.Utilizadores.AnyAsync(u =>
                u.Id == utilizadorId
            );*/

            /*!await _utilizadorRepo.ExisteAsync(utilizadorId)*/
            if (!await _contexto.Utilizadores.AnyAsync(u => u.Id == utilizadorId))
            {
                resultado.Sucesso = false;
                resultado.MensagemErro = "Este Utilizador Não Existe";
                return resultado;
            }

            var publicacao = await _publicacaoRepo.SelecionarAsync(publicacaoId);
            if (publicacao == null)
            {
                resultado.Sucesso = false;
                resultado.MensagemErro = "Esta Publicacao Não Existe";
                return resultado;
            }

            var comentario = dto.ParaComentarioDeComentarioDto(publicacaoId, utilizadorId);
            comentario.DataComentario = DateTime.Now;

            // 🔥 Mantém o contador sincronizado na raiz do post
            publicacao.QuantidadeComentarios++;

            await _comentarioRepo.AdicionarAsync(comentario);
            await _comentarioRepo.SalvarAsync();
            await _publicacaoRepo.SalvarAsync();

            resultado.ComentarioDto = comentario.ToComentarioDto();
            return resultado;
        }

        public async Task<ComentarioResultadoDto> EditarAsync(
            int id,
            EditarComentarioRequestDto dto
        )
        {
            var resultado = new ComentarioResultadoDto();
            var comentario = await _comentarioRepo.ObterPorIdAsync(id);
            

            if (comentario == null)
            {
                resultado.Sucesso = false;
                resultado.NaoEncontrado = true;
                resultado.MensagemErro = "Comentário não encontrado";
                return resultado;
            }

            comentario.ConteudoComentario = dto.ConteudoComentario;
            comentario.DataActualizacao = DateTime.Now;

            await _comentarioRepo.SalvarAsync();

            resultado.ComentarioDto = comentario.ToComentarioDto();
            return resultado;
        }

        public async Task<ComentarioResultadoDto> ExcluirAsync(int id)
        {
            var resultado = new ComentarioResultadoDto();
            var comentario = await _comentarioRepo.ObterPorIdAsync(id);

            if (comentario == null)
            {
                resultado.Sucesso = false;
                resultado.NaoEncontrado = true;
                resultado.MensagemErro = "Comentário não encontrado";
                return resultado;
            }

            // 🔥 Decrementa o contador da publicação original se ela existir
            var publicacao = await _publicacaoRepo.SelecionarAsync(comentario.PublicacaoId);
            if (publicacao != null && publicacao.QuantidadeComentarios > 0)
            {
                publicacao.QuantidadeComentarios--;
                await _publicacaoRepo.SalvarAsync();
            }

            _comentarioRepo.Remover(comentario);
            await _comentarioRepo.SalvarAsync();

            return resultado;
        }

        public async Task<List<ComentarioDto>> ListarAsync(int id)
        {
            var comentarios = await _comentarioRepo.ListarPorPublicacaoIdAsync(id);

            var listaComentariosDto = new List<ComentarioDto>();

            foreach(var com in comentarios)
            {
                var comentarioDto = com.ToComentarioDto();
                listaComentariosDto.Add(comentarioDto);
            }

            return listaComentariosDto;
        }
    }
}
