using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Jobs.Commands.CreateJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.SubmitJob;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobById;
using GiapTech.BlouseHiding.Application.Ops.Commands.ModerateJob;
using GiapTech.BlouseHiding.Application.Ops.Commands.VerifyOrganization;
using GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingOrganizations;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Ops;

public class VerifyOrganizationTests : TestBase
{
    private static async Task<(Guid ownerId, Guid orgId)> SeedPendingOrgAsync(string ownerEmail)
    {
        var ownerId = await TestApp.RunAsUserAsync(ownerEmail, "Testing1234!", [Roles.Employer]);
        var org = new Organization { Name = "Bệnh viện Chờ duyệt", OrgType = OrganizationType.BenhVienTu, VerifyStatus = OrganizationVerifyStatus.Pending };
        org.AddOwner(ownerId);
        await TestApp.AddAsync(org);
        return (ownerId, org.Id);
    }

    [Test]
    public async Task Verify_Pending_Organization_Should_Succeed()
    {
        var (_, orgId) = await SeedPendingOrgAsync($"owner1-{Guid.NewGuid():N}@test.local");

        var modId = await TestApp.RunAsUserAsync($"mod1-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);

        var pending = await TestApp.SendAsync<List<PendingOrganizationDto>>(new GetPendingOrganizationsQuery());
        pending.ShouldContain(o => o.Id == orgId);

        await TestApp.SendAsync(new VerifyOrganizationCommand { OrganizationId = orgId, VerifiedBy = modId, Action = OrganizationVerifyAction.Verify });

        var org = await TestApp.FindAsync<Organization>(orgId);
        org!.VerifyStatus.ShouldBe(OrganizationVerifyStatus.Verified);
    }

    [Test]
    public async Task Reject_Pending_Organization_Without_Reason_Should_Throw()
    {
        var (_, orgId) = await SeedPendingOrgAsync($"owner2-{Guid.NewGuid():N}@test.local");
        var modId = await TestApp.RunAsUserAsync($"mod2-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync(new VerifyOrganizationCommand { OrganizationId = orgId, VerifiedBy = modId, Action = OrganizationVerifyAction.Reject, RejectReason = null }));
    }

    [Test]
    public async Task Suspend_Verified_Organization_Should_Auto_Suspend_Published_Jobs()
    {
        var specialty = new Specialty { Code = $"sp-{Guid.NewGuid():N}", Name = "Nội tổng quát" };
        var location = new Location { Name = "TP.HCM" };
        var package = new JobPackage { Tier = JobPackageTier.Free, Name = "Free", DurationDays = 7, Price = 0 };
        await TestApp.AddAsync(specialty);
        await TestApp.AddAsync(location);
        await TestApp.AddAsync(package);

        var ownerId = await TestApp.RunAsUserAsync($"owner3-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);
        var org = new Organization { Name = "Bệnh viện Bị rút xác thực", OrgType = OrganizationType.BenhVienTu, VerifyStatus = OrganizationVerifyStatus.Verified };
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

        var modId = await TestApp.RunAsUserAsync($"mod3-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);
        await TestApp.SendAsync(new ModerateJobCommand { JobId = jobId, Approved = true });

        var jobBefore = await TestApp.SendAsync<Application.Jobs.JobDto>(new GetJobByIdQuery { JobId = jobId, RequestingUserId = ownerId });
        jobBefore.Status.ShouldBe("Published");

        await TestApp.SendAsync(new VerifyOrganizationCommand { OrganizationId = org.Id, VerifiedBy = modId, Action = OrganizationVerifyAction.Suspend });

        var job = await TestApp.FindAsync<Job>(jobId);
        job!.Status.ShouldBe(JobStatus.Suspended);

        var orgAfter = await TestApp.FindAsync<Organization>(org.Id);
        orgAfter!.VerifyStatus.ShouldBe(OrganizationVerifyStatus.Suspended);
    }

    [Test]
    public async Task Suspend_NonVerified_Organization_Should_Throw()
    {
        var (_, orgId) = await SeedPendingOrgAsync($"owner4-{Guid.NewGuid():N}@test.local");
        var modId = await TestApp.RunAsUserAsync($"mod4-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync(new VerifyOrganizationCommand { OrganizationId = orgId, VerifiedBy = modId, Action = OrganizationVerifyAction.Suspend }));
    }
}
