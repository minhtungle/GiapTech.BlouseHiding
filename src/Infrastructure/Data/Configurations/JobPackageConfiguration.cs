using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class JobPackageConfiguration : IEntityTypeConfiguration<JobPackage>
{
    public void Configure(EntityTypeBuilder<JobPackage> builder)
    {
        builder.Property(p => p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Price).HasColumnType("numeric(12,2)");
        builder.Property(p => p.Perks).HasColumnType("jsonb");

        builder.HasMany(p => p.Translations)
            .WithOne(t => t.JobPackage)
            .HasForeignKey(t => t.JobPackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
