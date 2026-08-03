namespace GiapTech.BlouseHiding.Application.Common.Interfaces;

public interface IOtpSender
{
    Task SendAsync(string email, string code, OtpPurpose purpose, CancellationToken cancellationToken);
}
