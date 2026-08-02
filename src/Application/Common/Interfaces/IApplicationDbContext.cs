namespace GiapTech.BlouseHiding.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Specialty> Specialties { get; }

    DbSet<SpecialtyTranslation> SpecialtyTranslations { get; }

    DbSet<Location> Locations { get; }

    DbSet<LocationTranslation> LocationTranslations { get; }

    DbSet<JobPackage> JobPackages { get; }

    DbSet<JobPackageTranslation> JobPackageTranslations { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
