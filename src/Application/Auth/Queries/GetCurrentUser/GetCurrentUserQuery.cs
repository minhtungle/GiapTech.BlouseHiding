using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Models;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery : IQuery<AuthUserDto>
{
    public Guid UserId { get; init; }
}

public class GetCurrentUserQueryHandler : IQueryHandler<GetCurrentUserQuery, AuthUserDto>
{
    private readonly IIdentityService _identityService;

    public GetCurrentUserQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async ValueTask<AuthUserDto> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        return await _identityService.FindByIdAsync(query.UserId)
            ?? throw new NotFoundException(nameof(query.UserId), query.UserId);
    }
}
