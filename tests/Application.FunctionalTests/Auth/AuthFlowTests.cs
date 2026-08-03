using GiapTech.BlouseHiding.Application.Auth.Commands.ForgotPassword;
using GiapTech.BlouseHiding.Application.Auth.Commands.Login;
using GiapTech.BlouseHiding.Application.Auth.Commands.Logout;
using GiapTech.BlouseHiding.Application.Auth.Commands.RefreshToken;
using GiapTech.BlouseHiding.Application.Auth.Commands.Register;
using GiapTech.BlouseHiding.Application.Auth.Commands.ResetPassword;
using GiapTech.BlouseHiding.Application.Auth.Commands.VerifyOtp;
using GiapTech.BlouseHiding.Domain.Constants;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Auth;

using RefreshTokenCommand = Application.Auth.Commands.RefreshToken.RefreshTokenCommand;

public class AuthFlowTests : TestBase
{
    [Test]
    public async Task Register_Then_VerifyOtp_Then_Login_Should_Succeed()
    {
        var email = $"candidate-{Guid.NewGuid():N}@test.local";

        var userId = await TestApp.SendAsync<Guid>(new RegisterCommand
        {
            Email = email,
            Password = "Testing1234!",
            Role = Roles.Candidate,
        });

        userId.ShouldNotBe(Guid.Empty);

        var code = CapturingOtpSender.LastCode;
        code.ShouldNotBeNullOrWhiteSpace();

        var verified = await TestApp.SendAsync<bool>(new VerifyOtpCommand { Email = email, Code = code! });
        verified.ShouldBeTrue();

        var login = await TestApp.SendAsync<LoginResult>(new LoginCommand { Email = email, Password = "Testing1234!" });
        login.AccessToken.ShouldNotBeNullOrWhiteSpace();
        login.RefreshToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task VerifyOtp_With_Wrong_Code_Should_Return_False()
    {
        var email = $"candidate-{Guid.NewGuid():N}@test.local";
        await TestApp.SendAsync<Guid>(new RegisterCommand { Email = email, Password = "Testing1234!", Role = Roles.Candidate });

        var verified = await TestApp.SendAsync<bool>(new VerifyOtpCommand { Email = email, Code = "000000" });

        verified.ShouldBeFalse();
    }

    [Test]
    public async Task Login_With_Wrong_Password_Should_Throw_Unauthorized()
    {
        var email = $"candidate-{Guid.NewGuid():N}@test.local";
        await TestApp.SendAsync<Guid>(new RegisterCommand { Email = email, Password = "Testing1234!", Role = Roles.Candidate });

        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync<LoginResult>(new LoginCommand { Email = email, Password = "WrongPassword1!" }));
    }

    [Test]
    public async Task Refresh_Should_Rotate_Token_And_Old_Token_Becomes_Invalid()
    {
        var email = $"candidate-{Guid.NewGuid():N}@test.local";
        await TestApp.SendAsync<Guid>(new RegisterCommand { Email = email, Password = "Testing1234!", Role = Roles.Candidate });
        var login = await TestApp.SendAsync<LoginResult>(new LoginCommand { Email = email, Password = "Testing1234!" });

        var refreshed = await TestApp.SendAsync<RefreshTokenResponse>(new RefreshTokenCommand { RefreshToken = login.RefreshToken });
        refreshed.AccessToken.ShouldNotBeNullOrWhiteSpace();
        refreshed.RefreshToken.ShouldNotBe(login.RefreshToken);

        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync<RefreshTokenResponse>(new RefreshTokenCommand { RefreshToken = login.RefreshToken }));
    }

    [Test]
    public async Task Logout_Should_Revoke_Refresh_Token()
    {
        var email = $"candidate-{Guid.NewGuid():N}@test.local";
        await TestApp.SendAsync<Guid>(new RegisterCommand { Email = email, Password = "Testing1234!", Role = Roles.Candidate });
        var login = await TestApp.SendAsync<LoginResult>(new LoginCommand { Email = email, Password = "Testing1234!" });

        await TestApp.SendAsync(new LogoutCommand { RefreshToken = login.RefreshToken });

        await Should.ThrowAsync<UnauthorizedAccessException>(async () =>
            await TestApp.SendAsync<RefreshTokenResponse>(new RefreshTokenCommand { RefreshToken = login.RefreshToken }));
    }

    [Test]
    public async Task ForgotPassword_Then_ResetPassword_Should_Allow_Login_With_New_Password()
    {
        var email = $"candidate-{Guid.NewGuid():N}@test.local";
        await TestApp.SendAsync<Guid>(new RegisterCommand { Email = email, Password = "Testing1234!", Role = Roles.Candidate });

        await TestApp.SendAsync(new ForgotPasswordCommand { Email = email });
        var code = CapturingOtpSender.LastCode!;

        await TestApp.SendAsync(new ResetPasswordCommand { Email = email, Code = code, NewPassword = "NewPassword1234!" });

        var login = await TestApp.SendAsync<LoginResult>(new LoginCommand { Email = email, Password = "NewPassword1234!" });
        login.AccessToken.ShouldNotBeNullOrWhiteSpace();
    }

    [Test]
    public async Task ForgotPassword_For_Unknown_Email_Should_Not_Throw()
    {
        await Should.NotThrowAsync(async () =>
            await TestApp.SendAsync(new ForgotPasswordCommand { Email = "unknown@test.local" }));
    }
}
