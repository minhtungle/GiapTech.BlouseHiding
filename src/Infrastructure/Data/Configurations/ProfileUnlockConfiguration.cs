using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class ProfileUnlockConfiguration : IEntityTypeConfiguration<ProfileUnlock>
{
    public void Configure(EntityTypeBuilder<ProfileUnlock> builder)
    {
        builder.HasIndex(u => new { u.OrganizationId, u.CandidateId }).IsUnique();
    }
}
