using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace NzolaWebAPI.Models
{
    [Table("tb_FicheirosConteudo")]
    public class FicheiroConteudo
    {
        [Key]
        public int Id { get; set; }
        
        public int PublicacaoId { get; set; }
        
        [ForeignKey("PublicacaoId")]
        public Publicacao Publicacao { get; set; }
        
        public string CaminhoFicheiro { get; set; }  // Caminho relativo (ex: "/uploads/guid.jpg")
        public string TipoMime { get; set; }         // Tipo MIME (ex: "image/jpeg")
        public long TamanhoBytes { get; set; }       // Tamanho do ficheiro
        public DateTime DataUpload { get; set; } = DateTime.UtcNow;
    }
}
