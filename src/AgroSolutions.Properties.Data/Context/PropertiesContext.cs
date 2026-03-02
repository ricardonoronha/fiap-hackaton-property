using AgroSolutions.Properties.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class PropertiesContext : DbContext
{
    public PropertiesContext(DbContextOptions<PropertiesContext> options)
        : base(options)
    {
    }

    public DbSet<Property> Properties { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<Culture> Cultures { get; set; }

    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PropertiesContext).Assembly);
    }
}