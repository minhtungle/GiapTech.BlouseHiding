using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Employers.Commands.AcceptInvitation;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/database/ERD-CHI-TIET.md mục 2.3 (organization_invitations) — người được mời phải đăng
// nhập (đăng ký trước nếu chưa có tài khoản, email khớp lời mời) rồi mới gọi endpoint này.
public class Invitations : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/invitations";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(AcceptInvitation, "{token}/accept").RequireAuthorization();
    }

    public static async Task AcceptInvitation(string token, ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var email = principal.FindFirstValue(ClaimTypes.Email)!;

        var command = new AcceptInvitationCommand
        {
            UserId = userId,
            UserEmail = email,
            Token = token,
        };

        await sender.Send(command, cancellationToken);
    }
}
