using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Jobs;
using GiapTech.BlouseHiding.Application.Jobs.Commands.CloseJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.CreateJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.RenewJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.SubmitJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.UpdateJob;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobById;
using GiapTech.BlouseHiding.Application.Jobs.Queries.SearchJobs;
using GiapTech.BlouseHiding.Application.Ops.Commands.ModerateJob;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Jobs;

public class JobLifecycleTests : TestBase
{
    private static async Task<(Specialty specialty, Location location, JobPackage freePackage)> SeedCatalogAsync()
    {
        var specialty = new Specialty { Code = $"sp-{Guid.NewGuid():N}", Name = "Nội tổng quát" };
        var location = new Location { Name = "TP.HCM" };
        var package = new JobPackage { Tier = JobPackageTier.Free, Name = "Free", DurationDays = 7, Price = 0 };

        await TestApp.AddAsync(specialty);
        await TestApp.AddAsync(location);
        await TestApp.AddAsync(package);

        return (specialty, location, package);
    }

    private static async Task<(Guid ownerId, Guid orgId)> SeedVerifiedOrgAsync(string ownerEmail)
    {
        var ownerId = await TestApp.RunAsUserAsync(ownerEmail, "Testing1234!", [Roles.Employer]);

        var org = new Organization { Name = "Bệnh viện Test", OrgType = OrganizationType.BenhVienTu, VerifyStatus = OrganizationVerifyStatus.Verified };
        org.AddOwner(ownerId);
        await TestApp.AddAsync(org);

        return (ownerId, org.Id);
    }

    [Test]
    public async Task Create_Then_Submit_Free_Then_Moderate_Should_Publish()
    {
        var (specialty, location, package) = await SeedCatalogAsync();
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner1-{Guid.NewGuid():N}@test.local");

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Title = "Điều dưỡng ICU",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.FullTime,
            LocationId = location.Id,
            Description = "Mô tả công việc",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });

        await TestApp.RunAsUserAsync($"mod1-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);
        await TestApp.SendAsync(new ModerateJobCommand { JobId = jobId, Approved = true });

        var job = await TestApp.SendAsync<JobDto>(new GetJobByIdQuery { JobId = jobId });
        job.Status.ShouldBe("Published");

        var searchResults = await TestApp.SendAsync<List<JobDto>>(new SearchJobsQuery());
        searchResults.ShouldContain(j => j.Id == jobId);
    }

    [Test]
    public async Task Moderate_Approve_Should_Fail_When_Organization_Not_Verified()
    {
        var (specialty, location, package) = await SeedCatalogAsync();

        var ownerId = await TestApp.RunAsUserAsync($"owner2-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);
        var org = new Organization { Name = "Bệnh viện Chưa xác thực", OrgType = OrganizationType.PhongKham, VerifyStatus = OrganizationVerifyStatus.Pending };
        org.AddOwner(ownerId);
        await TestApp.AddAsync(org);

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = org.Id,
            Title = "Bác sĩ đa khoa",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.PartTime,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });

        await TestApp.RunAsUserAsync($"mod2-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync(new ModerateJobCommand { JobId = jobId, Approved = true }));
    }

    [Test]
    public async Task GetJobById_Draft_Should_Return_NotFound_For_Guest()
    {
        var (specialty, location, _) = await SeedCatalogAsync();
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner3-{Guid.NewGuid():N}@test.local");

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Title = "Kỹ thuật viên xét nghiệm",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.Locum,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        await Should.ThrowAsync<NotFoundException>(async () =>
            await TestApp.SendAsync<JobDto>(new GetJobByIdQuery { JobId = jobId, RequestingUserId = null }));
    }

    [Test]
    public async Task UpdateJob_After_Submit_Should_Throw()
    {
        var (specialty, location, package) = await SeedCatalogAsync();
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner4-{Guid.NewGuid():N}@test.local");

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Title = "Dược sĩ",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.Ctv,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync(new UpdateJobCommand
            {
                UserId = ownerId,
                JobId = jobId,
                Title = "Dược sĩ (sửa)",
                SpecialtyId = specialty.Id,
                EmploymentType = EmploymentType.Ctv,
                LocationId = location.Id,
                Description = "Mô tả sửa",
            }));
    }

    [Test]
    public async Task CloseJob_Then_Renew_Should_Create_New_Draft_Job()
    {
        var (specialty, location, package) = await SeedCatalogAsync();
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner5-{Guid.NewGuid():N}@test.local");

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Title = "Hộ sinh",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.TrucCa,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });
        await TestApp.RunAsUserAsync($"mod3-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);
        await TestApp.SendAsync(new ModerateJobCommand { JobId = jobId, Approved = true });

        TestApp.SetCurrentUser(ownerId, [Roles.Employer]);
        var renewedJobId = await TestApp.SendAsync<Guid>(new RenewJobCommand { UserId = ownerId, JobId = jobId });

        renewedJobId.ShouldNotBe(jobId);

        var oldJob = await TestApp.FindAsync<Job>(jobId);
        oldJob!.Status.ShouldBe(JobStatus.Closed);

        var renewedJob = await TestApp.FindAsync<Job>(renewedJobId);
        renewedJob!.Status.ShouldBe(JobStatus.Draft);
        renewedJob.Title.ShouldBe("Hộ sinh");
    }

    [Test]
    public async Task CloseJob_By_NonMember_Should_Throw_Forbidden()
    {
        var (specialty, location, _) = await SeedCatalogAsync();
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner6-{Guid.NewGuid():N}@test.local");

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Title = "Điều dưỡng",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.FullTime,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        var otherUserId = await TestApp.RunAsUserAsync($"other1-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync(new CloseJobCommand { UserId = otherUserId, JobId = jobId }));
    }
}
