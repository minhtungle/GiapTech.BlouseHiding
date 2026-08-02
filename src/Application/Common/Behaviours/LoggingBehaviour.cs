using GiapTech.BlouseHiding.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace GiapTech.BlouseHiding.Application.Common.Behaviours;

// Ghi log trước khi request đi vào handler — trước dùng MediatR IRequestPreProcessor,
// Mediator không có khái niệm tương đương nên gộp vào IPipelineBehavior thường.
public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IMessage
{
    private readonly ILogger _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public LoggingBehaviour(ILogger<TRequest> logger, IUser user, IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async ValueTask<TResponse> Handle(TRequest request, MessageHandlerDelegate<TRequest, TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _user.Id ?? string.Empty;
        string? userName = string.Empty;

        if (!string.IsNullOrEmpty(userId))
        {
            userName = await _identityService.GetUserNameAsync(userId);
        }

        _logger.LogInformation("GiapTech.BlouseHiding Request: {Name} {@UserId} {@UserName} {@Request}",
            requestName, userId, userName, request);

        return await next(request, cancellationToken);
    }
}
