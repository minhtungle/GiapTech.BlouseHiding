using System.Security.Claims;
using GiapTech.BlouseHiding.Application.Auth.Queries.GetCurrentUser;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Models;

namespace GiapTech.BlouseHiding.Web.Endpoints;

public class Users : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetMe, "me").RequireAuthorization();
        groupBuilder.MapDelete(DeleteMe, "me").RequireAuthorization();
    }

    public static async Task<AuthUserDto> GetMe(ClaimsPrincipal principal, ISender sender, CancellationToken cancellationToken)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await sender.Send(new GetCurrentUserQuery { UserId = userId }, cancellationToken);
    }

    public static async Task<IResult> DeleteMe(ClaimsPrincipal principal, IIdentityService identityService)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await identityService.DeleteUserAsync(userId);

        return result.Succeeded ? TypedResults.NoContent() : TypedResults.BadRequest(result.Errors);
    }
}
