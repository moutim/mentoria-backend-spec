using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests;

public class CriarMovimentoRequest
{
    [Required]
    [MaxLength(11)]
    public string UsuarioId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(11)]
    public string Remetente { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(11)]
    public string Destinatario { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; } = string.Empty;
    
    public int? CategoriaId { get; set; }
    
    [MaxLength(255)]
    public string? Descricao { get; set; }
    
    [Required]
    public decimal Valor { get; set; }
}
