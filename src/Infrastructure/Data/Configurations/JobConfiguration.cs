using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class JobConfiguration : IEntityTypeConfiguration<Job>
{
    public void Configure(EntityTypeBuilder<Job> builder)
    {
        builder.Property(j => j.Title).HasMaxLength(255).IsRequired();
        builder.Property(j => j.Description).IsRequired();

        builder.HasIndex(j => new { j.Status, j.SpecialtyId, j.LocationId });
        builder.HasIndex(j => new { j.OrganizationId, j.Status });

        builder.HasOne(j => j.Organization)
            .WithMany()
            .HasForeignKey(j => j.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Specialty)
            .WithMany()
            .HasForeignKey(j => j.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(j => j.Location)
            .WithMany()
            .HasForeignKey(j => j.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
