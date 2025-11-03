using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests;

public class CriarContaRequest
{
    [Required]
    [StringLength(100)]
    public string Numero { get; set; } = string.Empty;
    
    [Required]
    public decimal SaldoInicial { get; set; }
}
