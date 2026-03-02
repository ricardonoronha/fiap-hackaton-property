using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgroSolutions.Properties.Domain.Entities;

public class Field
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PropertyId { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid CultureId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Property Property { get; set; } = null!;
    public Culture Culture { get; set; } = null!;

    public virtual ICollection<Alert> Alerts { get; set; } = new List<Alert>();
}
