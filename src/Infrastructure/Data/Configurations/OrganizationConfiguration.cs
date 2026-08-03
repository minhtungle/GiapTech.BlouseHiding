using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
        builder.Property(o => o.LicenseNo).HasMaxLength(100);

        builder.HasMany(o => o.Members)
            .WithOne(m => m.Organization)
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
