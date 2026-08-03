namespace GiapTech.BlouseHiding.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Specialty> Specialties { get; }

    DbSet<SpecialtyTranslation> SpecialtyTranslations { get; }

    DbSet<Location> Locations { get; }

    DbSet<LocationTranslation> LocationTranslations { get; }

    DbSet<JobPackage> JobPackages { get; }

    DbSet<JobPackageTranslation> JobPackageTranslations { get; }

    DbSet<Organization> Organizations { get; }

    DbSet<EmployerMember> EmployerMembers { get; }

    DbSet<OrganizationInvitation> OrganizationInvitations { get; }

    DbSet<RefreshToken> RefreshTokens { get; }

    DbSet<OtpCode> OtpCodes { get; }

    DbSet<CandidateProfile> CandidateProfiles { get; }

    DbSet<License> Licenses { get; }

    DbSet<ProfileSpecialty> ProfileSpecialties { get; }

    DbSet<Job> Jobs { get; }

    DbSet<JobApplication> Applications { get; }

    DbSet<ApplicationNote> ApplicationNotes { get; }

    DbSet<ApplicationStageHistory> ApplicationStageHistories { get; }

    DbSet<CreditWallet> CreditWallets { get; }

    DbSet<CreditTransaction> CreditTransactions { get; }

    DbSet<ProfileUnlock> ProfileUnlocks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    // Bọc toàn bộ thao tác trong 1 DB transaction — dùng cho luồng phải atomic xuyên nhiều
    // SaveChangesAsync/ExecuteUpdateAsync (vd trừ Credit + tạo ProfileUnlock, CLAUDE.md mục 4 quy tắc
    // bất di bất dịch #2). Che giấu chi tiết EF Core/Npgsql khỏi Application layer.
    Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken);
}
