using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Entities;

public class Movimento
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int ContaId { get; set; }
    
    [Required]
    public decimal Valor { get; set; }
    
    [Required]
    public TipoMovimento Tipo { get; set; }
    
    [Required]
    public DateTime DataMovimento { get; set; }
    
    public virtual Conta Conta { get; set; } = null!;
}
