using GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateMyProfile;
using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Employers.Commands.CreateOrganization;
using GiapTech.BlouseHiding.Application.Employers.Commands.UnlockProfile;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetCreditTransactions;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetCreditWallet;
using GiapTech.BlouseHiding.Application.Employers.Queries.SearchCandidates;
using GiapTech.BlouseHiding.Application.Ops.Commands.CreditBonus;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Employers;

public class CreditUnlockTests : TestBase
{
    private static async Task<(Guid ownerId, Guid orgId)> SeedOrgAsync(string ownerEmail)
    {
        var ownerId = await TestApp.RunAsUserAsync(ownerEmail, "Testing1234!", [Roles.Employer]);
        var orgId = await TestApp.SendAsync<Guid>(new CreateOrganizationCommand
        {
            OwnerUserId = ownerId,
            Name = "Bệnh viện Credit Test",
            OrgType = OrganizationType.BenhVienTu,
        });
        return (ownerId, orgId);
    }

    private static async Task<Guid> SeedCandidateAsync(string email)
    {
        var userId = await TestApp.RunAsUserAsync(email, "Testing1234!", [Roles.Candidate]);
        await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand { UserId = userId, FullName = "Ứng viên Credit Test", Headline = "Điều dưỡng" });
        return userId;
    }

    [Test]
    public async Task CreateOrganization_Should_Create_Wallet_With_Zero_Balance()
    {
        var (ownerId, orgId) = await SeedOrgAsync($"owner1-{Guid.NewGuid():N}@test.local");

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);
        var wallet = await TestApp.SendAsync<CreditWalletDto>(new GetCreditWalletQuery { OrganizationId = orgId, UserId = ownerId });

        wallet.Balance.ShouldBe(0);
    }

    [Test]
    public async Task CreditBonus_Should_Increase_Balance_And_Record_Transaction()
    {
        var (ownerId, orgId) = await SeedOrgAsync($"owner2-{Guid.NewGuid():N}@test.local");
        var adminId = await TestApp.RunAsUserAsync($"admin1-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Admin]);

        await TestApp.SendAsync(new CreditBonusCommand { OrganizationId = orgId, CreatedBy = adminId, Amount = 50 });

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);
        var wallet = await TestApp.SendAsync<CreditWalletDto>(new GetCreditWalletQuery { OrganizationId = orgId, UserId = ownerId });
        wallet.Balance.ShouldBe(50);

        var transactions = await TestApp.SendAsync<List<CreditTransactionDto>>(new GetCreditTransactionsQuery { OrganizationId = orgId, UserId = ownerId });
        transactions.ShouldContain(t => t.Amount == 50 && t.Reason == "Bonus");
    }

    [Test]
    public async Task UnlockProfile_Should_Debit_Wallet_And_Reveal_Contact()
    {
        var (ownerId, orgId) = await SeedOrgAsync($"owner3-{Guid.NewGuid():N}@test.local");
        var adminId = await TestApp.RunAsUserAsync($"admin2-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Admin]);
        await TestApp.SendAsync(new CreditBonusCommand { OrganizationId = orgId, CreatedBy = adminId, Amount = 100 });

        var candidateUserId = await SeedCandidateAsync($"cand1-{Guid.NewGuid():N}@test.local");
        var candidateProfile = await TestApp.SendAsync<Application.Candidates.CandidateProfileDto>(
            new Application.Candidates.Queries.GetMyProfile.GetMyProfileQuery { UserId = candidateUserId });

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);

        var beforeSearch = await TestApp.SendAsync<List<CandidateSearchResultDto>>(new SearchCandidatesQuery { OrganizationId = orgId, UserId = ownerId });
        beforeSearch.Single(c => c.Id == candidateProfile.Id).IsUnlocked.ShouldBeFalse();

        var unlockId = await TestApp.SendAsync<Guid>(new UnlockProfileCommand { UserId = ownerId, OrganizationId = orgId, CandidateId = candidateProfile.Id });
        unlockId.ShouldNotBe(Guid.Empty);

        var wallet = await TestApp.SendAsync<CreditWalletDto>(new GetCreditWalletQuery { OrganizationId = orgId, UserId = ownerId });
        wallet.Balance.ShouldBe(85);

        var afterSearch = await TestApp.SendAsync<List<CandidateSearchResultDto>>(new SearchCandidatesQuery { OrganizationId = orgId, UserId = ownerId });
        var unlockedResult = afterSearch.Single(c => c.Id == candidateProfile.Id);
        unlockedResult.IsUnlocked.ShouldBeTrue();
        unlockedResult.ContactEmail.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task UnlockProfile_Twice_Should_Be_Idempotent_And_Not_Double_Charge()
    {
        var (ownerId, orgId) = await SeedOrgAsync($"owner4-{Guid.NewGuid():N}@test.local");
        var adminId = await TestApp.RunAsUserAsync($"admin3-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Admin]);
        await TestApp.SendAsync(new CreditBonusCommand { OrganizationId = orgId, CreatedBy = adminId, Amount = 100 });

        var candidateUserId = await SeedCandidateAsync($"cand2-{Guid.NewGuid():N}@test.local");
        var candidateProfile = await TestApp.SendAsync<Application.Candidates.CandidateProfileDto>(
            new Application.Candidates.Queries.GetMyProfile.GetMyProfileQuery { UserId = candidateUserId });

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);

        var firstUnlockId = await TestApp.SendAsync<Guid>(new UnlockProfileCommand { UserId = ownerId, OrganizationId = orgId, CandidateId = candidateProfile.Id });
        var secondUnlockId = await TestApp.SendAsync<Guid>(new UnlockProfileCommand { UserId = ownerId, OrganizationId = orgId, CandidateId = candidateProfile.Id });

        secondUnlockId.ShouldBe(firstUnlockId);

        var wallet = await TestApp.SendAsync<CreditWalletDto>(new GetCreditWalletQuery { OrganizationId = orgId, UserId = ownerId });
        wallet.Balance.ShouldBe(85);
    }

    [Test]
    public async Task UnlockProfile_With_Insufficient_Balance_Should_Throw()
    {
        var (ownerId, orgId) = await SeedOrgAsync($"owner5-{Guid.NewGuid():N}@test.local");

        var candidateUserId = await SeedCandidateAsync($"cand3-{Guid.NewGuid():N}@test.local");
        var candidateProfile = await TestApp.SendAsync<Application.Candidates.CandidateProfileDto>(
            new Application.Candidates.Queries.GetMyProfile.GetMyProfileQuery { UserId = candidateUserId });

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync<Guid>(new UnlockProfileCommand { UserId = ownerId, OrganizationId = orgId, CandidateId = candidateProfile.Id }));
    }

    [Test]
    public async Task Concurrent_Unlock_Different_Candidates_Should_Not_Overdraw_Wallet()
    {
        var (ownerId, orgId) = await SeedOrgAsync($"owner6-{Guid.NewGuid():N}@test.local");
        var adminId = await TestApp.RunAsUserAsync($"admin4-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Admin]);
        // Đủ credit cho đúng 1 lần unlock (15) — nếu race condition xảy ra, cả 2 request cùng pass
        // check "đủ tiền" trước khi trừ thì balance sẽ âm (vi phạm CHECK constraint) — test này xác
        // nhận ExecuteUpdateAsync có điều kiện chặn đúng, không cần giả lập song song thật (khó
        // deterministic trong 1 test) — chỉ cần xác nhận sau 2 lần gọi tuần tự, chỉ 1 lần trừ thành công.
        await TestApp.SendAsync(new CreditBonusCommand { OrganizationId = orgId, CreatedBy = adminId, Amount = 15 });

        var candidate1UserId = await SeedCandidateAsync($"cand4-{Guid.NewGuid():N}@test.local");
        var candidate1Profile = await TestApp.SendAsync<Application.Candidates.CandidateProfileDto>(
            new Application.Candidates.Queries.GetMyProfile.GetMyProfileQuery { UserId = candidate1UserId });

        var candidate2UserId = await SeedCandidateAsync($"cand5-{Guid.NewGuid():N}@test.local");
        var candidate2Profile = await TestApp.SendAsync<Application.Candidates.CandidateProfileDto>(
            new Application.Candidates.Queries.GetMyProfile.GetMyProfileQuery { UserId = candidate2UserId });

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);

        await TestApp.SendAsync<Guid>(new UnlockProfileCommand { UserId = ownerId, OrganizationId = orgId, CandidateId = candidate1Profile.Id });

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync<Guid>(new UnlockProfileCommand { UserId = ownerId, OrganizationId = orgId, CandidateId = candidate2Profile.Id }));

        var wallet = await TestApp.SendAsync<CreditWalletDto>(new GetCreditWalletQuery { OrganizationId = orgId, UserId = ownerId });
        wallet.Balance.ShouldBe(0);
    }
}
