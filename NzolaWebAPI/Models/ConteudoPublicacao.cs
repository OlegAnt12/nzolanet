using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NzolaWebAPI.Models.Enums;

namespace NzolaWebAPI.Models
{
    [Table("tb_ConteudoPublicacao")]
    public class ConteudoPublicacao
    { 
        [Key]
        public int Id {get; set;}
        public int PublicacaoId { get; set; }
        [ForeignKey("PublicacaoId")]
        public Publicacao Publicacao {get; set;}
        public string Conteudo {get; set;}
        public TipoConteudo TipoConteudo { get; set; }
        public int Ordem { get; set; }
    }
}