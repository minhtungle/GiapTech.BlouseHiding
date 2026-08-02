using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class SpecialtyTranslationConfiguration : IEntityTypeConfiguration<SpecialtyTranslation>
{
    public void Configure(EntityTypeBuilder<SpecialtyTranslation> builder)
    {
        builder.Property(t => t.Locale).HasMaxLength(5).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(255).IsRequired();
        builder.HasIndex(t => new { t.SpecialtyId, t.Locale }).IsUnique();
    }
}
