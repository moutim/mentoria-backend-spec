using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Requests;

public class CriarMovimentoRequest
{
    [Required]
    public int ContaId { get; set; }
    
    [Required]
    public decimal Valor { get; set; }
    
    [Required]
    public TipoMovimento Tipo { get; set; }
}
