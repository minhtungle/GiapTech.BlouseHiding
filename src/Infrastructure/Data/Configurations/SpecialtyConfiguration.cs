using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class SpecialtyConfiguration : IEntityTypeConfiguration<Specialty>
{
    public void Configure(EntityTypeBuilder<Specialty> builder)
    {
        builder.Property(s => s.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => s.Code).IsUnique();
        builder.Property(s => s.Name).HasMaxLength(255).IsRequired();

        builder.HasOne(s => s.Parent)
            .WithMany(s => s.Children)
            .HasForeignKey(s => s.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Translations)
            .WithOne(t => t.Specialty)
            .HasForeignKey(t => t.SpecialtyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
