using System.Reflection;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiapTech.BlouseHiding.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<SpecialtyTranslation> SpecialtyTranslations => Set<SpecialtyTranslation>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<LocationTranslation> LocationTranslations => Set<LocationTranslation>();

    public DbSet<JobPackage> JobPackages => Set<JobPackage>();

    public DbSet<JobPackageTranslation> JobPackageTranslations => Set<JobPackageTranslation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
