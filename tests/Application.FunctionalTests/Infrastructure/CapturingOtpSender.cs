using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.FunctionalTests.Infrastructure;

// Thay LoggingOtpSender thật trong test — capture mã OTP để test tự verify được, không phải
// gửi email/SMS thật (xem docs/nghiep-vu/TIEN-DO-DU-AN.md — driver OTP hiện là giả lập nội bộ).
public class CapturingOtpSender : IOtpSender
{
    public static string? LastCode { get; private set; }

    public Task SendAsync(string email, string code, OtpPurpose purpose, CancellationToken cancellationToken)
    {
        LastCode = code;
        return Task.CompletedTask;
    }
}
