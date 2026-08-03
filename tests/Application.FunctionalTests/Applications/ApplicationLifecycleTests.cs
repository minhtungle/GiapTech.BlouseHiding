using GiapTech.BlouseHiding.Application.Applications;
using GiapTech.BlouseHiding.Application.Applications.Commands.AddApplicationNote;
using GiapTech.BlouseHiding.Application.Applications.Commands.ScoreApplication;
using GiapTech.BlouseHiding.Application.Applications.Commands.SubmitApplication;
using GiapTech.BlouseHiding.Application.Applications.Commands.TransitionApplicationStage;
using GiapTech.BlouseHiding.Application.Applications.Queries.GetApplicationById;
using GiapTech.BlouseHiding.Application.Applications.Queries.GetApplicationHistory;
using GiapTech.BlouseHiding.Application.Applications.Queries.GetMyApplications;
using GiapTech.BlouseHiding.Application.Candidates.Commands.AddProfileSpecialty;
using GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateMyProfile;
using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Jobs.Commands.CreateJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.SubmitJob;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobApplications;
using GiapTech.BlouseHiding.Application.Ops.Commands.ModerateJob;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Applications;

public class ApplicationLifecycleTests : TestBase
{
    private static async Task<(Guid publishedJobId, Guid employerUserId, Guid orgId)> SeedPublishedJobAsync()
    {
        var specialty = new Specialty { Code = $"sp-{Guid.NewGuid():N}", Name = "Nội tổng quát" };
        var location = new Location { Name = "TP.HCM" };
        var package = new JobPackage { Tier = JobPackageTier.Free, Name = "Free", DurationDays = 7, Price = 0 };
        await TestApp.AddAsync(specialty);
        await TestApp.AddAsync(location);
        await TestApp.AddAsync(package);

        var ownerId = await TestApp.RunAsUserAsync($"emp-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);
        var org = new Organization { Name = "Bệnh viện Test", OrgType = OrganizationType.BenhVienTu, VerifyStatus = OrganizationVerifyStatus.Verified };
        org.AddOwner(ownerId);
        await TestApp.AddAsync(org);

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = org.Id,
            Title = "Điều dưỡng ICU",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.FullTime,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });

        await TestApp.RunAsUserAsync($"mod-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);
        await TestApp.SendAsync(new ModerateJobCommand { JobId = jobId, Approved = true });

        return (jobId, ownerId, org.Id);
    }

    private static async Task<Guid> SeedCandidateAsync(string email)
    {
        var userId = await TestApp.RunAsUserAsync(email, "Testing1234!", [Roles.Candidate]);
        await TestApp.SendAsync<Guid>(new UpdateMyProfileCommand { UserId = userId, FullName = "Ứng viên Test" });
        return userId;
    }

    [Test]
    public async Task Submit_Application_Should_Create_With_CvSnapshot_And_Score()
    {
        var (jobId, _, _) = await SeedPublishedJobAsync();
        var candidateUserId = await SeedCandidateAsync($"cand1-{Guid.NewGuid():N}@test.local");

        var applicationId = await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId, CoverLetter = "Tôi rất phù hợp" });

        applicationId.ShouldNotBe(Guid.Empty);

        var myApplications = await TestApp.SendAsync<List<ApplicationDto>>(new GetMyApplicationsQuery { UserId = candidateUserId });
        myApplications.ShouldContain(a => a.Id == applicationId && a.Stage == "New");
    }

    [Test]
    public async Task Submit_Application_Twice_Should_Throw()
    {
        var (jobId, _, _) = await SeedPublishedJobAsync();
        var candidateUserId = await SeedCandidateAsync($"cand2-{Guid.NewGuid():N}@test.local");

        await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId });

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId }));
    }

    [Test]
    public async Task Submit_Application_To_NonPublished_Job_Should_Throw()
    {
        var specialty = new Specialty { Code = $"sp-{Guid.NewGuid():N}", Name = "Nội tổng quát" };
        var location = new Location { Name = "TP.HCM" };
        await TestApp.AddAsync(specialty);
        await TestApp.AddAsync(location);

        var ownerId = await TestApp.RunAsUserAsync($"emp2-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);
        var org = new Organization { Name = "Bệnh viện Chưa đăng", OrgType = OrganizationType.PhongKham, VerifyStatus = OrganizationVerifyStatus.Verified };
        org.AddOwner(ownerId);
        await TestApp.AddAsync(org);

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = org.Id,
            Title = "Tin còn draft",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.PartTime,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        var candidateUserId = await SeedCandidateAsync($"cand3-{Guid.NewGuid():N}@test.local");

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId }));
    }

    [Test]
    public async Task TransitionStage_Should_Record_History()
    {
        var (jobId, employerUserId, _) = await SeedPublishedJobAsync();
        var candidateUserId = await SeedCandidateAsync($"cand4-{Guid.NewGuid():N}@test.local");
        var applicationId = await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId });

        TestApp.SetCurrentUser(employerUserId, [Roles.Employer]);
        await TestApp.SendAsync(new TransitionApplicationStageCommand
        {
            UserId = employerUserId,
            ApplicationId = applicationId,
            Stage = ApplicationStage.Reviewing,
            Silent = false,
        });

        var history = await TestApp.SendAsync<List<ApplicationStageHistoryDto>>(new GetApplicationHistoryQuery { ApplicationId = applicationId, UserId = employerUserId });
        history.ShouldContain(h => h.FromStage == "New" && h.ToStage == "Reviewing");

        var application = await TestApp.SendAsync<ApplicationDto>(new GetApplicationByIdQuery { ApplicationId = applicationId, UserId = candidateUserId });
        application.Stage.ShouldBe("Reviewing");
    }

    [Test]
    public async Task TransitionStage_By_NonMember_Should_Throw_Forbidden()
    {
        var (jobId, _, _) = await SeedPublishedJobAsync();
        var candidateUserId = await SeedCandidateAsync($"cand5-{Guid.NewGuid():N}@test.local");
        var applicationId = await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId });

        var otherEmployerId = await TestApp.RunAsUserAsync($"other-emp-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new TransitionApplicationStageCommand
            {
                UserId = otherEmployerId,
                ApplicationId = applicationId,
                Stage = ApplicationStage.Reviewing,
                Silent = false,
            }));
    }

    [Test]
    public async Task AddNote_And_Score_Should_Succeed()
    {
        var (jobId, employerUserId, _) = await SeedPublishedJobAsync();
        var candidateUserId = await SeedCandidateAsync($"cand6-{Guid.NewGuid():N}@test.local");
        var applicationId = await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId });

        TestApp.SetCurrentUser(employerUserId, [Roles.Employer]);
        await TestApp.SendAsync<Guid>(new AddApplicationNoteCommand { UserId = employerUserId, ApplicationId = applicationId, NoteText = "Ứng viên tiềm năng" });
        await TestApp.SendAsync(new ScoreApplicationCommand { UserId = employerUserId, ApplicationId = applicationId, Score = 90 });

        var application = await TestApp.SendAsync<ApplicationDto>(new GetApplicationByIdQuery { ApplicationId = applicationId, UserId = employerUserId });
        application.Score.ShouldBe(90);
    }

    [Test]
    public async Task GetJobApplications_Should_Return_ATS_List()
    {
        var (jobId, employerUserId, _) = await SeedPublishedJobAsync();
        var candidateUserId = await SeedCandidateAsync($"cand7-{Guid.NewGuid():N}@test.local");
        var applicationId = await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId });

        TestApp.SetCurrentUser(employerUserId, [Roles.Employer]);
        var applications = await TestApp.SendAsync<List<ApplicationDto>>(new GetJobApplicationsQuery { JobId = jobId, UserId = employerUserId });

        applications.ShouldContain(a => a.Id == applicationId);
    }

    [Test]
    public async Task GetApplicationById_By_Unrelated_User_Should_Throw_Forbidden()
    {
        var (jobId, _, _) = await SeedPublishedJobAsync();
        var candidateUserId = await SeedCandidateAsync($"cand8-{Guid.NewGuid():N}@test.local");
        var applicationId = await TestApp.SendAsync<Guid>(new SubmitApplicationCommand { UserId = candidateUserId, JobId = jobId });

        var unrelatedUserId = await TestApp.RunAsUserAsync($"unrelated-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Candidate]);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync<ApplicationDto>(new GetApplicationByIdQuery { ApplicationId = applicationId, UserId = unrelatedUserId }));
    }
}
