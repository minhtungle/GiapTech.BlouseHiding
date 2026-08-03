using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Applications;
using GiapTech.BlouseHiding.Application.Applications.Commands.AddApplicationNote;
using GiapTech.BlouseHiding.Application.Applications.Commands.ScoreApplication;
using GiapTech.BlouseHiding.Application.Applications.Commands.TransitionApplicationStage;
using GiapTech.BlouseHiding.Application.Applications.Queries.GetApplicationById;
using GiapTech.BlouseHiding.Application.Applications.Queries.GetApplicationHistory;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 8.
public class Applications : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetApplicationById, "{applicationId:guid}").RequireAuthorization();
        groupBuilder.MapPatch(TransitionStage, "{applicationId:guid}/stage").RequireAuthorization();
        groupBuilder.MapPost(AddNote, "{applicationId:guid}/notes").RequireAuthorization();
        groupBuilder.MapPatch(Score, "{applicationId:guid}/score").RequireAuthorization();
        groupBuilder.MapGet(GetHistory, "{applicationId:guid}/history").RequireAuthorization();
    }

    public static async Task<ApplicationDto> GetApplicationById(Guid applicationId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetApplicationByIdQuery { ApplicationId = applicationId, UserId = CurrentUserId(principal) };
        return await sender.Send(query, cancellationToken);
    }

    public static async Task TransitionStage(Guid applicationId, TransitionStageRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new TransitionApplicationStageCommand
        {
            UserId = CurrentUserId(principal),
            ApplicationId = applicationId,
            Stage = request.Stage,
            Silent = request.Silent,
            RejectedReason = request.RejectedReason,
        };

        await sender.Send(command, cancellationToken);
    }

    public static async Task<Guid> AddNote(Guid applicationId, AddNoteRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new AddApplicationNoteCommand { UserId = CurrentUserId(principal), ApplicationId = applicationId, NoteText = request.NoteText };
        return await sender.Send(command, cancellationToken);
    }

    public static async Task Score(Guid applicationId, ScoreRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var command = new ScoreApplicationCommand { UserId = CurrentUserId(principal), ApplicationId = applicationId, Score = request.Score };
        await sender.Send(command, cancellationToken);
    }

    public static async Task<List<ApplicationStageHistoryDto>> GetHistory(Guid applicationId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetApplicationHistoryQuery { ApplicationId = applicationId, UserId = CurrentUserId(principal) };
        return await sender.Send(query, cancellationToken);
    }

    private static Guid CurrentUserId(ClaimsPrincipal principal) =>
        Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
}

public record TransitionStageRequest(ApplicationStage Stage, bool Silent, string? RejectedReason);

public record AddNoteRequest(string NoteText);

public record ScoreRequest(int Score);
