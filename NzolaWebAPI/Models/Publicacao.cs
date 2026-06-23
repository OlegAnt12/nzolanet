using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    [Table("tb_Publicacoes")]
    public class Publicacao
    {
        [Key]
        public int Id { get; set; }
        public int AutorId { get; set; }

        [ForeignKey("AutorId")]
        public Utilizador Utilizador { get; set; }

        [Required]
        public string Texto { get; set; } = string.Empty;

        // Uma publicação pode conter múltiplas mídias (imagens/vídeos)
        public List<FicheiroConteudo> Ficheiros { get; set; } = new List<FicheiroConteudo>();

        [Column(TypeName = "nvarchar(11)")]
        [DefaultValue("Existente")]
        public EstadoExistenciaLogica Existencia { get; set; } = EstadoExistenciaLogica.Existente;

        public int QuantidadeBazes { get; set; }
        public int QuantidadeComentarios { get; set; }
        public DateTime DataPublicacao { get; set; } = DateTime.Now;
        public DateTime DataAtualizacaoPublicacao { get; set; }
        public List<Comentario> Comentarios { get; set; } = new List<Comentario>();
        public List<Baze> Bazes { get; set; } = new List<Baze>();
    }
}
