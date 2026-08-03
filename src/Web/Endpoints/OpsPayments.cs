using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Ops.Commands.ConfirmPayment;
using GiapTech.BlouseHiding.Application.Ops.Commands.RejectPayment;
using GiapTech.BlouseHiding.Application.Ops.Queries.GetPendingPayments;

namespace GiapTech.BlouseHiding.Web.Endpoints;

public class OpsPayments : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/ops/payments";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetPendingPayments, "").RequireAuthorization();
        groupBuilder.MapPost(ConfirmPayment, "{paymentId:guid}/confirm").RequireAuthorization();
        groupBuilder.MapPost(RejectPayment, "{paymentId:guid}/reject").RequireAuthorization();
    }

    public static async Task<List<PendingPaymentDto>> GetPendingPayments(ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetPendingPaymentsQuery(), cancellationToken);
    }

    public static async Task ConfirmPayment(Guid paymentId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await sender.Send(new ConfirmPaymentCommand { PaymentId = paymentId, ConfirmedBy = userId }, cancellationToken);
    }

    public static async Task RejectPayment(Guid paymentId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await sender.Send(new RejectPaymentCommand { PaymentId = paymentId, ConfirmedBy = userId }, cancellationToken);
    }
}
