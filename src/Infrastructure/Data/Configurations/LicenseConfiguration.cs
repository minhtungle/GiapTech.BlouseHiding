using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class LicenseConfiguration : IEntityTypeConfiguration<License>
{
    public void Configure(EntityTypeBuilder<License> builder)
    {
        builder.Property(l => l.LicenseNo).HasMaxLength(100).IsRequired();
        builder.Property(l => l.IssuedBy).HasMaxLength(255).IsRequired();
        builder.Property(l => l.DocumentUrl).IsRequired();

        builder.HasIndex(l => l.VerifyStatus);
    }
}
