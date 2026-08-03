using GiapTech.BlouseHiding.Application.Common.Interfaces;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Infrastructure;

// Thay LoggingInvitationSender thật trong test — capture token lời mời để test tự verify được,
// không phải gửi email thật (xem docs/nghiep-vu/TIEN-DO-DU-AN.md — driver hiện là giả lập nội bộ).
public class CapturingInvitationSender : IInvitationSender
{
    public static string? LastToken { get; private set; }

    public Task SendAsync(string email, string organizationName, string token, CancellationToken cancellationToken)
    {
        LastToken = token;
        return Task.CompletedTask;
    }
}
