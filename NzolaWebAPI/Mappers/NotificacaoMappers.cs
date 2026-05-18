using NzolaWebAPI.Models;
using NzolaWebAPI.Dtos.Notificacao;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace NzolaWebAPI.Mappers
{
    public static class NotificacaoMappers
    {
        public static NotificacaoDto ToNotificacaoDto (this Notificacao notificacaoModel)
        {
            return new NotificacaoDto
            {
                Id = notificacaoModel.Id,
                UtilizadorId = notificacaoModel.UtilizadorId,
                Tipo = notificacaoModel.Tipo,
                OrigemId = notificacaoModel.OrigemId,
                Mensagem = notificacaoModel.Mensagem,
                Lida = notificacaoModel.Lida,
                CriadoEm = notificacaoModel.CriadoEm

            };
        }
        
    }
}