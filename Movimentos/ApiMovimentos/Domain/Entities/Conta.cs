using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Conta
{
    public Conta()
    {
        Movimentos = new HashSet<Movimento>();
    }
    
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Numero { get; set; } = string.Empty;
    
    [Required]
    public decimal Saldo { get; set; }
    
    public virtual ICollection<Movimento> Movimentos { get; set; }
}
