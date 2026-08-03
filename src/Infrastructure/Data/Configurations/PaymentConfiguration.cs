using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(p => p.Amount).HasColumnType("numeric(12,2)");
        builder.Property(p => p.ReferenceCode).HasMaxLength(20);
        builder.Property(p => p.ProviderTxnId).HasMaxLength(255);

        builder.HasIndex(p => p.ReferenceCode).IsUnique();
        builder.HasIndex(p => p.OrganizationId);

        builder.HasOne(p => p.Organization)
            .WithMany()
            .HasForeignKey(p => p.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
