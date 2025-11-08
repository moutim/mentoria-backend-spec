using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("movimentos")]
public class Movimento
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("usuario_id")]
    [MaxLength(11)]
    public string UsuarioId { get; set; } = string.Empty;
    
    [Required]
    [Column("remetente")]
    [MaxLength(11)]
    public string Remetente { get; set; } = string.Empty;
    
    [Required]
    [Column("destinatario")]
    [MaxLength(11)]
    public string Destinatario { get; set; } = string.Empty;
    
    [Required]
    [Column("tipo")]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;
    
    [Column("categoria_id")]
    public int? CategoriaId { get; set; }
    
    [Column("descricao")]
    [MaxLength(255)]
    public string? Descricao { get; set; }
    
    [Required]
    [Column("valor")]
    public decimal Valor { get; set; }
    
    [Column("criado_em")]
    public DateTime CriadoEm { get; set; }
    
    // Navigation property
    [ForeignKey("CategoriaId")]
    public Categoria? Categoria { get; set; }
}
