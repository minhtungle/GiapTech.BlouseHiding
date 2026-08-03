using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class EmployerMemberConfiguration : IEntityTypeConfiguration<EmployerMember>
{
    public void Configure(EntityTypeBuilder<EmployerMember> builder)
    {
        builder.HasIndex(m => new { m.OrganizationId, m.UserId }).IsUnique();
    }
}
