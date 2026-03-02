using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroSolutions.Properties.Domain.Entities;

public class Property
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string FarmerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Field> Fields { get; set; } = new List<Field>();
}
