using System.Reflection;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GiapTech.BlouseHiding.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Specialty> Specialties => Set<Specialty>();

    public DbSet<SpecialtyTranslation> SpecialtyTranslations => Set<SpecialtyTranslation>();

    public DbSet<Location> Locations => Set<Location>();

    public DbSet<LocationTranslation> LocationTranslations => Set<LocationTranslation>();

    public DbSet<JobPackage> JobPackages => Set<JobPackage>();

    public DbSet<JobPackageTranslation> JobPackageTranslations => Set<JobPackageTranslation>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<EmployerMember> EmployerMembers => Set<EmployerMember>();

    public DbSet<OrganizationInvitation> OrganizationInvitations => Set<OrganizationInvitation>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<OtpCode> OtpCodes => Set<OtpCode>();

    public DbSet<CandidateProfile> CandidateProfiles => Set<CandidateProfile>();

    public DbSet<License> Licenses => Set<License>();

    public DbSet<ProfileSpecialty> ProfileSpecialties => Set<ProfileSpecialty>();

    public DbSet<Job> Jobs => Set<Job>();

    public DbSet<JobApplication> Applications => Set<JobApplication>();

    public DbSet<ApplicationNote> ApplicationNotes => Set<ApplicationNote>();

    public DbSet<ApplicationStageHistory> ApplicationStageHistories => Set<ApplicationStageHistory>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
