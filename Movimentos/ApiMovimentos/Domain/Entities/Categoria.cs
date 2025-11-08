using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

[Table("categorias")]
public class Categoria
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Required]
    [Column("nome")]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;
    
    // Navigation property
    public ICollection<Movimento> Movimentos { get; set; } = new List<Movimento>();
}

