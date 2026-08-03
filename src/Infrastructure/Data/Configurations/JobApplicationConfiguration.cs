using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
{
    public void Configure(EntityTypeBuilder<JobApplication> builder)
    {
        builder.ToTable("Applications");

        builder.Property(a => a.CvSnapshot).IsRequired();

        builder.HasIndex(a => new { a.JobId, a.Stage });
        builder.HasIndex(a => a.CandidateId);
        builder.HasIndex(a => new { a.JobId, a.CandidateId }).IsUnique();

        builder.HasOne(a => a.Job)
            .WithMany()
            .HasForeignKey(a => a.JobId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Candidate)
            .WithMany()
            .HasForeignKey(a => a.CandidateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(a => a.StageHistory)
            .WithOne()
            .HasForeignKey(h => h.ApplicationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
