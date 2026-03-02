namespace AgroSolutions.Properties.Application.DTOs;

public record PropertyOutputDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Location { get; init; }
    public string FarmerId { get; set; }
    public DateTime CreatedAt { get; init; }
}
