using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class JobPurchaseConfiguration : IEntityTypeConfiguration<JobPurchase>
{
    public void Configure(EntityTypeBuilder<JobPurchase> builder)
    {
        builder.HasIndex(p => p.JobId);
        builder.HasIndex(p => p.OrganizationId);
    }
}
