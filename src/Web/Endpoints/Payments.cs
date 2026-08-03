using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Payments.Commands.CreateCreditTopupPayment;
using GiapTech.BlouseHiding.Application.Payments.Commands.CreateJobPackagePayment;
using GiapTech.BlouseHiding.Application.Payments.Queries.GetPaymentById;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 6-7 — MVP chỉ có quy trình thủ công (ADR-0003), không có
// cổng thanh toán tự động/webhook.
public class Payments : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/payments";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateJobPackagePayment, "job-package").RequireAuthorization();
        groupBuilder.MapPost(CreateCreditTopupPayment, "credit-topup").RequireAuthorization();
        groupBuilder.MapGet(GetPaymentById, "{paymentId:guid}").RequireAuthorization();
    }

    public static async Task<PaymentInstructionsDto> CreateJobPackagePayment(
        CreateJobPackagePaymentRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateJobPackagePaymentCommand
        {
            UserId = userId,
            JobId = request.JobId,
            PackageId = request.PackageId,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task<PaymentInstructionsDto> CreateCreditTopupPayment(
        CreateCreditTopupPaymentRequest request, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var command = new CreateCreditTopupPaymentCommand
        {
            UserId = userId,
            OrganizationId = request.OrganizationId,
            CreditAmount = request.CreditAmount,
        };

        return await sender.Send(command, cancellationToken);
    }

    public static async Task<PaymentDto> GetPaymentById(Guid paymentId, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetPaymentByIdQuery { UserId = userId, PaymentId = paymentId }, cancellationToken);
    }
}

public record CreateJobPackagePaymentRequest(Guid JobId, Guid PackageId);
public record CreateCreditTopupPaymentRequest(Guid OrganizationId, int CreditAmount);
