using AgroSolutions.Properties.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroSolutions.Properties.Data.Configurations
{
    internal class AlertConfig : IEntityTypeConfiguration<Alert>
    {
        public void Configure(EntityTypeBuilder<Alert> builder)
        {
            builder.ToTable("alerts");
            builder.HasKey(a => a.Id);

            builder
                .Property(a => a.Type)
                .HasConversion<string>()
                .IsRequired();
            
            builder
                .Property(a => a.StartDate)
                .IsRequired();
            
            builder
                .Property(a => a.EndDate)
                .IsRequired(false);

            builder.Property(a => a.Active)
                .IsRequired();

            builder
                .HasOne(a => a.Field)
                .WithMany(f => f.Alerts)
                .HasForeignKey(a => a.FieldId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
