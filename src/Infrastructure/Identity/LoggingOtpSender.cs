using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace GiapTech.BlouseHiding.Infrastructure.Identity;

// Driver giả lập nội bộ cho MVP — log OTP thay vì gửi email/SMS thật (chưa chốt nhà cung cấp,
// xem docs/nghiep-vu/TIEN-DO-DU-AN.md). Thay bằng driver SMTP/SMS thật sau mà không đổi luồng
// nghiệp vụ vì Command chỉ phụ thuộc IOtpSender.
public class LoggingOtpSender : IOtpSender
{
    private readonly ILogger<LoggingOtpSender> _logger;

    public LoggingOtpSender(ILogger<LoggingOtpSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string email, string code, OtpPurpose purpose, CancellationToken cancellationToken)
    {
        _logger.LogInformation("OTP giả lập ({Purpose}) cho {Email}: {Code}", purpose, email, code);
        return Task.CompletedTask;
    }
}
