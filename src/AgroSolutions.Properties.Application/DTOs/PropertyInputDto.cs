using System.ComponentModel.DataAnnotations;

namespace AgroSolutions.Properties.Application.DTOs;

// ===== PROPERTY DTOs =====

public record PropertyInputDto
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(500)]
    public string? Location { get; init; }

    public string? FarmerId { get; init; }
}
