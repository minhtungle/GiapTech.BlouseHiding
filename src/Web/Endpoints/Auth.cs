using GiapTech.BlouseHiding.Application.Auth.Commands.ForgotPassword;
using GiapTech.BlouseHiding.Application.Auth.Commands.Login;
using GiapTech.BlouseHiding.Application.Auth.Commands.Logout;
using GiapTech.BlouseHiding.Application.Auth.Commands.RefreshToken;
using GiapTech.BlouseHiding.Application.Auth.Commands.Register;
using GiapTech.BlouseHiding.Application.Auth.Commands.ResetPassword;
using GiapTech.BlouseHiding.Application.Auth.Commands.VerifyOtp;

namespace GiapTech.BlouseHiding.Web.Endpoints;

// Xem docs/backend/API-DESIGN.md mục 2.
public class Auth : IEndpointGroup
{
    public static string? RoutePrefix => "/api/v1/auth";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(Register, "register");
        groupBuilder.MapPost(VerifyOtp, "verify-otp");
        groupBuilder.MapPost(Login, "login");
        groupBuilder.MapPost(Refresh, "refresh");
        groupBuilder.MapPost(Logout, "logout").RequireAuthorization();
        groupBuilder.MapPost(ForgotPassword, "forgot-password");
        groupBuilder.MapPost(ResetPassword, "reset-password");
    }

    public static async Task<Guid> Register(RegisterCommand command, ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }

    public static async Task<bool> VerifyOtp(VerifyOtpCommand command, ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }

    public static async Task<LoginResult> Login(LoginCommand command, ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }

    public static async Task<RefreshTokenResponse> Refresh(RefreshTokenCommand command, ISender sender, CancellationToken cancellationToken)
    {
        return await sender.Send(command, cancellationToken);
    }

    public static async Task Logout(LogoutCommand command, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
    }

    public static async Task ForgotPassword(ForgotPasswordCommand command, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
    }

    public static async Task ResetPassword(ResetPasswordCommand command, ISender sender, CancellationToken cancellationToken)
    {
        await sender.Send(command, cancellationToken);
    }
}
