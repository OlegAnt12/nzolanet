using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NzolaWebAPI.Models
{
    [Table("tb_RefreshTokens")]
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        public int UtilizadorId { get; set; }

        [ForeignKey(nameof(UtilizadorId))]
        public Utilizador Utilizador { get; set; } = null!;

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RevokedAt { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        [NotMapped]
        public bool IsActive => RevokedAt == null && !IsExpired;
    }
}
