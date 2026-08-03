using GiapTech.BlouseHiding.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GiapTech.BlouseHiding.Infrastructure.Data.Configurations;

public class ApplicationNoteConfiguration : IEntityTypeConfiguration<ApplicationNote>
{
    public void Configure(EntityTypeBuilder<ApplicationNote> builder)
    {
        builder.Property(n => n.NoteText).IsRequired();
        builder.HasIndex(n => n.ApplicationId);
    }
}
