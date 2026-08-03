using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;

namespace GiapTech.BlouseHiding.Application.Jobs.Commands.CreateJob;

[Authorize(Roles = Roles.Employer)]
public record CreateJobCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public string Title { get; init; } = string.Empty;
    public Guid SpecialtyId { get; init; }
    public Domain.Enums.EmploymentType EmploymentType { get; init; }
    public int? SalaryMin { get; init; }
    public int? SalaryMax { get; init; }
    public bool SalaryNegotiable { get; init; }
    public Guid LocationId { get; init; }
    public string? AddressDetail { get; init; }
    public bool RequiredLicense { get; init; } = true;
    public int MinExperienceYears { get; init; }
    public string Description { get; init; } = string.Empty;
    public string? Requirements { get; init; }
    public string? Benefits { get; init; }
}

public class CreateJobCommandValidator : AbstractValidator<CreateJobCommand>
{
    public CreateJobCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.SpecialtyId).NotEmpty();
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.MinExperienceYears).GreaterThanOrEqualTo(0);
    }
}

public class CreateJobCommandHandler : ICommandHandler<CreateJobCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(CreateJobCommand command, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == command.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var job = new Domain.Entities.Job
        {
            OrganizationId = command.OrganizationId,
            Title = command.Title,
            SpecialtyId = command.SpecialtyId,
            EmploymentType = command.EmploymentType,
            SalaryMin = command.SalaryMin,
            SalaryMax = command.SalaryMax,
            SalaryNegotiable = command.SalaryNegotiable,
            LocationId = command.LocationId,
            AddressDetail = command.AddressDetail,
            RequiredLicense = command.RequiredLicense,
            MinExperienceYears = command.MinExperienceYears,
            Description = command.Description,
            Requirements = command.Requirements,
            Benefits = command.Benefits,
            CreatedByUserId = command.UserId,
        };

        _context.Jobs.Add(job);
        await _context.SaveChangesAsync(cancellationToken);

        return job.Id;
    }
}
