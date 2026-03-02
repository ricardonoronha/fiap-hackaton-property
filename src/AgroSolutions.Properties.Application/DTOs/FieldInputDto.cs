using System.ComponentModel.DataAnnotations;

namespace AgroSolutions.Properties.Application.DTOs;

// ===== FIELD DTOs =====

public record FieldInputDto
{
    [Required(ErrorMessage = "PropertyId is required")]
    public Guid PropertyId { get; init; }

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(255)]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage = "Culture is required")]
    [MaxLength(100)]
    public Guid CultureId { get; init; }
}
