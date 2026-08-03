using GiapTech.BlouseHiding.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GiapTech.BlouseHiding.Infrastructure.Identity;

// Driver giả lập nội bộ cho MVP — log token lời mời thay vì gửi email thật, cùng quyết định với
// LoggingOtpSender (chưa chốt nhà cung cấp SMTP). Thay driver thật sau không đổi luồng nghiệp vụ vì
// Command chỉ phụ thuộc IInvitationSender.
public class LoggingInvitationSender : IInvitationSender
{
    private readonly ILogger<LoggingInvitationSender> _logger;

    public LoggingInvitationSender(ILogger<LoggingInvitationSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string email, string organizationName, string token, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Lời mời tham gia tổ chức giả lập cho {Email} vào {OrganizationName}: token={Token}",
            email, organizationName, token);
        return Task.CompletedTask;
    }
}
