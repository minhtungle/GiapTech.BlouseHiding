using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;

namespace GiapTech.BlouseHiding.Application.Employers.Commands.CreateOrganization;

[Authorize(Roles = Roles.Employer)]
public record CreateOrganizationCommand : ICommand<Guid>
{
    public Guid OwnerUserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public OrganizationType OrgType { get; init; }
    public string? LicenseNo { get; init; }
    public OrganizationSize? Size { get; init; }
}

public class CreateOrganizationCommandValidator : AbstractValidator<CreateOrganizationCommand>
{
    public CreateOrganizationCommandValidator()
    {
        RuleFor(x => x.OwnerUserId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255);
        RuleFor(x => x.OrgType).IsInEnum();
    }
}

public class CreateOrganizationCommandHandler : ICommandHandler<CreateOrganizationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateOrganizationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(CreateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = new Domain.Entities.Organization
        {
            Name = command.Name,
            OrgType = command.OrgType,
            LicenseNo = command.LicenseNo,
            Size = command.Size,
        };
        organization.AddOwner(command.OwnerUserId);

        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync(cancellationToken);

        return organization.Id;
    }
}
