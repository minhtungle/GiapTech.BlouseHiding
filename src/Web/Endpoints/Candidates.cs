using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Applications;
using GiapTech.BlouseHiding.Application.Applications.Queries.GetMyApplications;
using GiapTech.BlouseHiding.Application.Candidates;
using GiapTech.BlouseHiding.Application.Candidates.Commands.AddLicense;
using GiapTech.BlouseHiding.Application.Candidates.Commands.AddProfileSpecialty;
using GiapTech.BlouseHiding.Application.Candidates.Commands.DeleteLicense;
using GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateLicense;
using GiapTech.BlouseHiding.Application.Candidates.Commands.UpdateMyProfile;
using GiapTech.BlouseHiding.Application.Candidates.Queries.GetMyProfile;
using GiapTech.BlouseHiding.Application.Employers.Commands.UnlockProfile;
using GiapTech.BlouseHiding.Application.Employers.Queries.SearchCandidates;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 3, 7 (search/unlock).
public class Candidates : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetMyProfile, "me").RequireAuthorization();
        groupBuilder.MapPut(UpdateMyProfile, "me").RequireAuthorization();
        groupBuilder.MapPost(AddLicense, "me/licenses").RequireAuthorization();
        groupBuilder.MapPut(UpdateLicense, "me/licenses/{licenseId:guid}").RequireAuthorization();
        groupBuilder.MapDelete(DeleteLicense, "me/licenses/{licenseId:guid}").RequireAuthorization();
        groupBuilder.MapPost(AddSpecialty, "me/specialties").RequireAuthorization();
        groupBuilder.MapGet(GetMyApplications, "me/applications").RequireAuthorization();
        groupBuilder.MapGet(SearchCandidates, "search").RequireAuthorization();
        groupBuilder.MapPost(UnlockCandidate, "{candidateId:guid}/unlock").RequireAuthorization();
    }

    public static async Task<List<CandidateSearchResultDto>> SearchCandidates(
        Guid organizationId, Guid? specialty, Guid? location, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var query = new SearchCandidatesQuery
        {
            OrganizationId = organizationId,
            UserId = CurrentUserId(principal),
            SpecialtyId = specialty,
            LocationId = location,
        };

        return await sender.Send(query, cancellationToken);
    }

    public static async Task<Guid> UnlockCandidate(Guid candidateId, UnlockCandidateRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UnlockProfileCommand
        {
            UserId = CurrentUserId(principal),
            OrganizationId = request.OrganizationId,
            CandidateId = candidateId,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task<List<ApplicationDto>> GetMyApplications(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetMyApplicationsQuery { UserId = userId }, cancellationToken);
    }

    public static async Task<CandidateProfileDto> GetMyProfile(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetMyProfileQuery { UserId = CurrentUserId(principal) }, cancellationToken);
    }

    public static async Task<Guid> UpdateMyProfile(UpdateMyProfileRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UpdateMyProfileCommand
        {
            UserId = CurrentUserId(principal),
            FullName = request.FullName,
            Headline = request.Headline,
            Summary = request.Summary,
            Dob = request.Dob,
            Gender = request.Gender,
            Address = request.Address,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task<Guid> AddLicense(AddLicenseRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new AddLicenseCommand
        {
            UserId = CurrentUserId(principal),
            LicenseNo = request.LicenseNo,
            IssuedBy = request.IssuedBy,
            Scope = request.Scope,
            IssuedAt = request.IssuedAt,
            ExpiredAt = request.ExpiredAt,
            DocumentUrl = request.DocumentUrl,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task UpdateLicense(Guid licenseId, AddLicenseRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new UpdateLicenseCommand
        {
            UserId = CurrentUserId(principal),
            LicenseId = licenseId,
            LicenseNo = request.LicenseNo,
            IssuedBy = request.IssuedBy,
            Scope = request.Scope,
            IssuedAt = request.IssuedAt,
            ExpiredAt = request.ExpiredAt,
            DocumentUrl = request.DocumentUrl,
        };

        await sender.Send(command, cancellationToken);
    }

    public static async Task DeleteLicense(Guid licenseId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteLicenseCommand { UserId = CurrentUserId(principal), LicenseId = licenseId }, cancellationToken);
    }

    public static async Task<Guid> AddSpecialty(AddProfileSpecialtyRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new AddProfileSpecialtyCommand
        {
            UserId = CurrentUserId(principal),
            SpecialtyId = request.SpecialtyId,
            Level = request.Level,
        };

        return await sender.Send(command, cancellationToken);
    }

    private static Guid CurrentUserId(ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record UpdateMyProfileRequest(
    string FullName,
    string? Headline,
    string? Summary,
    DateOnly? Dob,
    Domain.Enums.Gender? Gender,
    string? Address);

public record AddLicenseRequest(
    string LicenseNo,
    string IssuedBy,
    string? Scope,
    DateOnly IssuedAt,
    DateOnly? ExpiredAt,
    string DocumentUrl);

public record AddProfileSpecialtyRequest(Guid SpecialtyId, Domain.Enums.SpecialtyLevel Level);

public record UnlockCandidateRequest(Guid OrganizationId);
