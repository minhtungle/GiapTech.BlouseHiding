using GiapTech.BlouseHiding.Application.Ops.Commands.ModerateJob;
using GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingJobs;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 11.
public class OpsJobs : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/ops/jobs";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetPendingJobs, "").RequireAuthorization();
        groupBuilder.MapPost(Moderate, "{jobId:guid}/moderate").RequireAuthorization();
    }

    public static async Task<List<PendingJobDto>> GetPendingJobs(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetPendingJobsQuery(), cancellationToken);
    }

    public static async Task Moderate(Guid jobId, ModerateJobRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var command = new ModerateJobCommand { JobId = jobId, Approved = request.Approved, RejectReason = request.RejectReason };
        await sender.Send(command, cancellationToken);
    }
}

public record ModerateJobRequest(bool Approved, string? RejectReason);
