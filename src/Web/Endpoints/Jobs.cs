using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Applications;
using GiapTech.BlouseHiding.Application.Applications.Commands.SubmitApplication;
using GiapTech.BlouseHiding.Application.Jobs;
using GiapTech.BlouseHiding.Application.Jobs.Commands.CloseJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.CreateJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.RenewJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.SubmitJob;
using GiapTech.BlouseHiding.Application.Jobs.Commands.UpdateJob;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobApplications;
using GiapTech.BlouseHiding.Application.Jobs.Queries.GetJobById;
using GiapTech.BlouseHiding.Application.Jobs.Queries.SearchJobs;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 5.
public class Jobs : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(Search, "");
        groupBuilder.MapGet(GetById, "{jobId:guid}");
        groupBuilder.MapPost(CreateJob, "").RequireAuthorization();
        groupBuilder.MapPut(UpdateJob, "{jobId:guid}").RequireAuthorization();
        groupBuilder.MapPost(Submit, "{jobId:guid}/submit").RequireAuthorization();
        groupBuilder.MapPost(Close, "{jobId:guid}/close").RequireAuthorization();
        groupBuilder.MapPost(Renew, "{jobId:guid}/renew").RequireAuthorization();
        groupBuilder.MapPost(Apply, "{jobId:guid}/applications").RequireAuthorization();
        groupBuilder.MapGet(GetApplications, "{jobId:guid}/applications").RequireAuthorization();
    }

    public static async Task<List<JobDto>> Search(
        Guid? specialty, Guid? location, EmploymentType? employmentType, int? salaryMin, string? keyword,
        ISender sender, CancellationToken cancellationToken)
    {
        var query = new SearchJobsQuery
        {
            SpecialtyId = specialty,
            LocationId = location,
            EmploymentType = employmentType,
            SalaryMin = salaryMin,
            Keyword = keyword,
        };

        return await sender.Send(query, cancellationToken);
    }

    public static async Task<JobDto> GetById(Guid jobId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetJobByIdQuery { JobId = jobId, RequestingUserId = TryGetUserId(principal) };
        return await sender.Send(query, cancellationToken);
    }

    public static async Task<Guid> CreateJob(CreateJobRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new CreateJobCommand
        {
            UserId = CurrentUserId(principal),
            OrganizationId = request.OrganizationId,
            Title = request.Title,
            SpecialtyId = request.SpecialtyId,
            EmploymentType = request.EmploymentType,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            SalaryNegotiable = request.SalaryNegotiable,
            LocationId = request.LocationId,
            AddressDetail = request.AddressDetail,
            RequiredLicense = request.RequiredLicense,
            MinExperienceYears = request.MinExperienceYears,
            Description = request.Description,
            Requirements = request.Requirements,
            Benefits = request.Benefits,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task UpdateJob(Guid jobId, UpdateJobRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UpdateJobCommand
        {
            UserId = CurrentUserId(principal),
            JobId = jobId,
            Title = request.Title,
            SpecialtyId = request.SpecialtyId,
            EmploymentType = request.EmploymentType,
            SalaryMin = request.SalaryMin,
            SalaryMax = request.SalaryMax,
            SalaryNegotiable = request.SalaryNegotiable,
            LocationId = request.LocationId,
            AddressDetail = request.AddressDetail,
            RequiredLicense = request.RequiredLicense,
            MinExperienceYears = request.MinExperienceYears,
            Description = request.Description,
            Requirements = request.Requirements,
            Benefits = request.Benefits,
        };

        await sender.Send(command, cancellationToken);
    }

    public static async Task Submit(Guid jobId, SubmitJobRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new SubmitJobCommand { UserId = CurrentUserId(principal), JobId = jobId, PackageId = request.PackageId }, cancellationToken);
    }

    public static async Task Close(Guid jobId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new CloseJobCommand { UserId = CurrentUserId(principal), JobId = jobId }, cancellationToken);
    }

    public static async Task<Guid> Renew(Guid jobId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new RenewJobCommand { UserId = CurrentUserId(principal), JobId = jobId }, cancellationToken);
    }

    public static async Task<Guid> Apply(Guid jobId, ApplyRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new SubmitApplicationCommand { UserId = CurrentUserId(principal), JobId = jobId, CoverLetter = request.CoverLetter };
        return await sender.Send(command, cancellationToken);
    }

    public static async Task<List<ApplicationDto>> GetApplications(Guid jobId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetJobApplicationsQuery { JobId = jobId, UserId = CurrentUserId(principal) };
        return await sender.Send(query, cancellationToken);
    }

    private static Guid CurrentUserId(ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static Guid? TryGetUserId(ClaimsPrincipal principal)
    {
        var claim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return claim is null ? null : Guid.Parse(claim);
    }
}

public record CreateJobRequest(
    Guid OrganizationId,
    string Title,
    Guid SpecialtyId,
    EmploymentType EmploymentType,
    int? SalaryMin,
    int? SalaryMax,
    bool SalaryNegotiable,
    Guid LocationId,
    string? AddressDetail,
    bool RequiredLicense,
    int MinExperienceYears,
    string Description,
    string? Requirements,
    string? Benefits);

public record UpdateJobRequest(
    string Title,
    Guid SpecialtyId,
    EmploymentType EmploymentType,
    int? SalaryMin,
    int? SalaryMax,
    bool SalaryNegotiable,
    Guid LocationId,
    string? AddressDetail,
    bool RequiredLicense,
    int MinExperienceYears,
    string Description,
    string? Requirements,
    string? Benefits);

public record SubmitJobRequest(Guid PackageId);

public record ApplyRequest(string? CoverLetter);
