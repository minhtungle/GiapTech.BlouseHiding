using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class ApplicationStageHistoryConfiguration : IEntityTypeConfiguration<ApplicationStageHistory>
{
    public void Configure(EntityTypeBuilder<ApplicationStageHistory> builder)
    {
        builder.HasIndex(h => h.ApplicationId);
    }
}
