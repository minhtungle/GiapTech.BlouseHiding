namespace GiapTech.BlouseHiding.Application.Common.Interfaces;

public interface IInvitationSender
{
    Task SendAsync(string email, string organizationName, string token, CancellationToken cancellationToken);
}
