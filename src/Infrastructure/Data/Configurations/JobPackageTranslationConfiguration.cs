using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class JobPackageTranslationConfiguration : IEntityTypeConfiguration<JobPackageTranslation>
{
    public void Configure(EntityTypeBuilder<JobPackageTranslation> builder)
    {
        builder.Property(t => t.Locale).HasMaxLength(5).IsRequired();
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.HasIndex(t => new { t.JobPackageId, t.Locale }).IsUnique();
    }
}
