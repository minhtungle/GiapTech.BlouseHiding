using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Employers.Queries.GetCreditWallet;
using GiapTech.BlouseHiding.Application.Jobs;
using GiapTech.BlouseHiding.Application.Jobs.Commands.CreateJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.SubmitJob;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobById;
using GiapTech.BlouseHiding.Application.Ops.Commands.ConfirmPayment;
using GiapTech.BlouseHiding.Application.Ops.Commands.RejectPayment;
using GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingPayments;
using GiapTech.BlouseHiding.Application.Payments.Commands.CreateCreditTopupPayment;
using GiapTech.BlouseHiding.Application.Payments.Commands.CreateJobPackagePayment;
using GiapTech.BlouseHiding.Application.Payments.Queries.GetPaymentById;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Entities;
using GiapTech.BlouseHiding.Domain.Enums;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Payments;

public class PaymentLifecycleTests : TestBase
{
    private static async Task<(Specialty specialty, Location location, JobPackage proPackage)> SeedCatalogAsync()
    {
        var specialty = new Specialty { Code = $"sp-{Guid.NewGuid():N}", Name = "Nội tổng quát" };
        var location = new Location { Name = "TP.HCM" };
        var package = new JobPackage { Tier = JobPackageTier.Pro, Name = "Pro", DurationDays = 30, Price = 990_000 };

        await TestApp.AddAsync(specialty);
        await TestApp.AddAsync(location);
        await TestApp.AddAsync(package);

        return (specialty, location, package);
    }

    private static async Task<(Guid ownerId, Guid orgId)> SeedVerifiedOrgAsync(string ownerEmail)
    {
        var ownerId = await TestApp.RunAsUserAsync(ownerEmail, "Testing1234!", [Roles.Employer]);

        var org = new Organization { Name = "Bệnh viện Test Payments", OrgType = OrganizationType.BenhVienTu, VerifyStatus = OrganizationVerifyStatus.Verified };
        org.AddOwner(ownerId);
        await TestApp.AddAsync(org);

        var wallet = new CreditWallet { OrganizationId = org.Id, Balance = 0 };
        await TestApp.AddAsync(wallet);

        return (ownerId, org.Id);
    }

    [Test]
    public async Task JobPackagePayment_Confirm_Should_Move_Job_To_Pending()
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
            Description = "Mô tả",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });

        var instructions = await TestApp.SendAsync<PaymentInstructionsDto>(new CreateJobPackagePaymentCommand
        {
            UserId = ownerId,
            JobId = jobId,
            PackageId = package.Id,
        });

        instructions.Amount.ShouldBe(990_000);
        instructions.ReferenceCode.ShouldStartWith("PAY-");

        await TestApp.RunAsUserAsync($"mod1-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);

        var pending = await TestApp.SendAsync<List<PendingPaymentDto>>(new GetPendingPaymentsQuery());
        pending.ShouldContain(p => p.Id == instructions.PaymentId && p.Type == "JobPackage");

        await TestApp.SendAsync(new ConfirmPaymentCommand { PaymentId = instructions.PaymentId, ConfirmedBy = Guid.NewGuid() });

        var job = await TestApp.SendAsync<JobDto>(new GetJobByIdQuery { JobId = jobId, RequestingUserId = ownerId });
        job.Status.ShouldBe("Pending");

        var payment = await TestApp.SendAsync<PaymentDto>(new GetPaymentByIdQuery { UserId = ownerId, PaymentId = instructions.PaymentId });
        payment.Status.ShouldBe("Success");
    }

    [Test]
    public async Task JobPackagePayment_Reject_Should_Move_Job_Back_To_Draft()
    {
        var (specialty, location, package) = await SeedCatalogAsync();
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner2-{Guid.NewGuid():N}@test.local");

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Title = "Bác sĩ đa khoa",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.PartTime,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });

        var instructions = await TestApp.SendAsync<PaymentInstructionsDto>(new CreateJobPackagePaymentCommand
        {
            UserId = ownerId,
            JobId = jobId,
            PackageId = package.Id,
        });

        await TestApp.RunAsUserAsync($"mod2-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);
        await TestApp.SendAsync(new RejectPaymentCommand { PaymentId = instructions.PaymentId, ConfirmedBy = Guid.NewGuid() });

        var job = await TestApp.SendAsync<JobDto>(new GetJobByIdQuery { JobId = jobId, RequestingUserId = ownerId });
        job.Status.ShouldBe("Draft");
    }

    [Test]
    public async Task CreateJobPackagePayment_Should_Throw_When_Already_Has_Pending_Payment()
    {
        var (specialty, location, package) = await SeedCatalogAsync();
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner3-{Guid.NewGuid():N}@test.local");

        var jobId = await TestApp.SendAsync<Guid>(new CreateJobCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            Title = "Kỹ thuật viên",
            SpecialtyId = specialty.Id,
            EmploymentType = EmploymentType.Locum,
            LocationId = location.Id,
            Description = "Mô tả",
        });

        await TestApp.SendAsync(new SubmitJobCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });
        await TestApp.SendAsync<PaymentInstructionsDto>(new CreateJobPackagePaymentCommand { UserId = ownerId, JobId = jobId, PackageId = package.Id });

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync<PaymentInstructionsDto>(new CreateJobPackagePaymentCommand
            {
                UserId = ownerId,
                JobId = jobId,
                PackageId = package.Id,
            }));
    }

    [Test]
    public async Task CreditTopupPayment_Confirm_Should_Increase_Wallet_Balance()
    {
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner4-{Guid.NewGuid():N}@test.local");

        var instructions = await TestApp.SendAsync<PaymentInstructionsDto>(new CreateCreditTopupPaymentCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            CreditAmount = 100,
        });

        instructions.Amount.ShouldBe(100_000);

        await TestApp.RunAsUserAsync($"mod3-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Moderator]);
        await TestApp.SendAsync(new ConfirmPaymentCommand { PaymentId = instructions.PaymentId, ConfirmedBy = Guid.NewGuid() });

        var wallet = await TestApp.SendAsync<CreditWalletDto>(new GetCreditWalletQuery { OrganizationId = orgId, UserId = ownerId });
        wallet.Balance.ShouldBe(100);
    }

    [Test]
    public async Task CreateCreditTopupPayment_Should_Throw_Forbidden_When_Not_Member()
    {
        var (_, orgId) = await SeedVerifiedOrgAsync($"owner5-{Guid.NewGuid():N}@test.local");
        var outsiderId = await TestApp.RunAsUserAsync($"outsider-{Guid.NewGuid():N}@test.local", "Testing1234!", [Roles.Employer]);

        await Should.ThrowAsync<ForbiddenAccessException>(async () =>
            await TestApp.SendAsync<PaymentInstructionsDto>(new CreateCreditTopupPaymentCommand
            {
                UserId = outsiderId,
                OrganizationId = orgId,
                CreditAmount = 50,
            }));
    }

    [Test]
    public async Task CreateCreditTopupPayment_Should_Throw_When_Already_Has_Pending_Topup()
    {
        var (ownerId, orgId) = await SeedVerifiedOrgAsync($"owner6-{Guid.NewGuid():N}@test.local");

        await TestApp.SendAsync<PaymentInstructionsDto>(new CreateCreditTopupPaymentCommand
        {
            UserId = ownerId,
            OrganizationId = orgId,
            CreditAmount = 50,
        });

        await Should.ThrowAsync<ValidationException>(async () =>
            await TestApp.SendAsync<PaymentInstructionsDto>(new CreateCreditTopupPaymentCommand
            {
                UserId = ownerId,
                OrganizationId = orgId,
                CreditAmount = 20,
            }));
    }
}
