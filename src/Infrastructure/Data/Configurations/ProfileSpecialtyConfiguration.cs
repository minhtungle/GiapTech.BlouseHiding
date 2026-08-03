using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class ProfileSpecialtyConfiguration : IEntityTypeConfiguration<ProfileSpecialty>
{
    public void Configure(EntityTypeBuilder<ProfileSpecialty> builder)
    {
        builder.HasIndex(s => new { s.ProfileId, s.SpecialtyId }).IsUnique();

        builder.HasOne(s => s.Specialty)
            .WithMany()
            .HasForeignKey(s => s.SpecialtyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
