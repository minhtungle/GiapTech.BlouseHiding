using GiapTech.BlouseHiding.Application.Candidates;
using GiapTech.BlouseHiding.Application.Candidates.Commands.AddLicense;
using GiapTech.BlouseHiding.Application.Candidates.Commands.AddProfileSpecialty;
using GiapTech.BlouseHiding.Application.Candidates.Commands.DeleteLicense;
using GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateLicense;
using GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateMyProfile;
using GiapTech.BlouseHiding.Application.Candidates.Queries.GetMyProfile;
using GiapTech.BlouseHiding.Application.Ops.Commands.VerifyLicense;
using GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingLicenses;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Candidates;

public class CandidateProfileTests : TestBase
{
    private static async Task<Specialty> SeedSpecialtyAsync()
    {
        var specialty = new Specialty { Code = $"sp-{Guid.NewGuid():N}", Name = "Nội tổng quát" };
        await TestApp.AddAsync(specialty);
        return specialty;
    }

    [Test]
    public async Task UpdateMyProfile_Should_Create_Profile_On_First_Call()
    {
        var userId = await TestApp.RunAsUserAsync("candidate1@test.local", "Testing1234!", [Roles.Candidate]);

        var profileId = await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand
        {
            UserId = userId,
            FullName = "Nguyễn Văn A",
            Headline = "Điều dưỡng ICU",
        });

        profileId.ShouldNotBe(Guid.Empty);

        var profile = await TestApp.SendAsync<CandidateProfileDto>(new GetMyProfileQuery { UserId = userId });
        profile.FullName.ShouldBe("Nguyễn Văn A");
        profile.CompletionPct.ShouldBeGreaterThan(0);
    }

    [Test]
    public async Task AddLicense_Then_VerifyLicense_Should_Update_Status()
    {
        var userId = await TestApp.RunAsUserAsync("candidate2@test.local", "Testing1234!", [Roles.Candidate]);
        await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand { UserId = userId, FullName = "Trần Thị B" });

        var licenseId = await TestApp.SendAsync<Guid>(new AddLicenseCommand
        {
            UserId = userId,
            LicenseNo = "CCHN-001",
            IssuedBy = "Sở Y tế TP.HCM",
            IssuedAt = new DateOnly(2020, 1, 1),
            DocumentUrl = "https://example.com/doc.pdf",
        });

        var opsUserId = await TestApp.RunAsUserAsync("moderator1@test.local", "Testing1234!", [Roles.Moderator]);

        var pending = await TestApp.SendAsync<List<PendingLicenseDto>>(new GetPendingLicensesQuery());
        pending.ShouldContain(l => l.Id == licenseId);

        await TestApp.SendAsync(new VerifyLicenseCommand { LicenseId = licenseId, VerifiedBy = opsUserId, Approved = true });

        var profile = await TestApp.SendAsync<CandidateProfileDto>(new GetMyProfileQuery { UserId = userId });
        profile.Licenses.Single(l => l.Id == licenseId).VerifyStatus.ShouldBe("Verified");
    }

    [Test]
    public async Task VerifyLicense_Rejected_Should_Require_Reason()
    {
        var userId = await TestApp.RunAsUserAsync("candidate3@test.local", "Testing1234!", [Roles.Candidate]);
        await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand { UserId = userId, FullName = "Lê Văn C" });

        var licenseId = await TestApp.SendAsync<Guid>(new AddLicenseCommand
        {
            UserId = userId,
            LicenseNo = "CCHN-002",
            IssuedBy = "Sở Y tế Hà Nội",
            IssuedAt = new DateOnly(2021, 1, 1),
            DocumentUrl = "https://example.com/doc2.pdf",
        });

        await TestApp.RunAsUserAsync("moderator2@test.local", "Testing1234!", [Roles.Moderator]);

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync(new VerifyLicenseCommand { LicenseId = licenseId, VerifiedBy = Guid.NewGuid(), Approved = false }));
    }

    [Test]
    public async Task UpdateLicense_After_Verified_Should_Throw()
    {
        var userId = await TestApp.RunAsUserAsync("candidate4@test.local", "Testing1234!", [Roles.Candidate]);
        await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand { UserId = userId, FullName = "Phạm Thị D" });

        var licenseId = await TestApp.SendAsync<Guid>(new AddLicenseCommand
        {
            UserId = userId,
            LicenseNo = "CCHN-003",
            IssuedBy = "Sở Y tế Đà Nẵng",
            IssuedAt = new DateOnly(2019, 1, 1),
            DocumentUrl = "https://example.com/doc3.pdf",
        });

        var opsUserId = await TestApp.RunAsUserAsync("moderator3@test.local", "Testing1234!", [Roles.Moderator]);
        await TestApp.SendAsync(new VerifyLicenseCommand { LicenseId = licenseId, VerifiedBy = opsUserId, Approved = true });

        TestApp.SetCurrentUser(userId, [Roles.Candidate]);

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync(new UpdateLicenseCommand
            {
                UserId = userId,
                LicenseId = licenseId,
                LicenseNo = "CCHN-003-edited",
                IssuedBy = "Sở Y tế Đà Nẵng",
                IssuedAt = new DateOnly(2019, 1, 1),
                DocumentUrl = "https://example.com/doc3.pdf",
            }));
    }

    [Test]
    public async Task DeleteLicense_Pending_Should_Succeed()
    {
        var userId = await TestApp.RunAsUserAsync("candidate5@test.local", "Testing1234!", [Roles.Candidate]);
        await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand { UserId = userId, FullName = "Hoàng Văn E" });

        var licenseId = await TestApp.SendAsync<Guid>(new AddLicenseCommand
        {
            UserId = userId,
            LicenseNo = "CCHN-004",
            IssuedBy = "Sở Y tế Cần Thơ",
            IssuedAt = new DateOnly(2022, 1, 1),
            DocumentUrl = "https://example.com/doc4.pdf",
        });

        await TestApp.SendAsync(new DeleteLicenseCommand { UserId = userId, LicenseId = licenseId });

        var profile = await TestApp.SendAsync<CandidateProfileDto>(new GetMyProfileQuery { UserId = userId });
        profile.Licenses.ShouldBeEmpty();
    }

    [Test]
    public async Task AddProfileSpecialty_Duplicate_Should_Throw()
    {
        var userId = await TestApp.RunAsUserAsync("candidate6@test.local", "Testing1234!", [Roles.Candidate]);
        await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand { UserId = userId, FullName = "Vũ Thị F" });

        var specialty = await SeedSpecialtyAsync();

        await TestApp.SendAsync<Guid>(new AddProfileSpecialtyCommand
        {
            UserId = userId,
            SpecialtyId = specialty.Id,
            Level = SpecialtyLevel.Senior,
        });

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync<Guid>(new AddProfileSpecialtyCommand
            {
                UserId = userId,
                SpecialtyId = specialty.Id,
                Level = SpecialtyLevel.Expert,
            }));
    }
}
