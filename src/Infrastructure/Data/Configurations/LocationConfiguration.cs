using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(l => l.Name).HasMaxLength(255).IsRequired();

        builder.HasOne(l => l.Parent)
            .WithMany(l => l.Children)
            .HasForeignKey(l => l.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.Translations)
            .WithOne(t => t.Location)
            .HasForeignKey(t => t.LocationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
