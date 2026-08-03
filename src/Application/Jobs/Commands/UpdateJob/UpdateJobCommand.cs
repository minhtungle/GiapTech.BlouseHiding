using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Jobs.Commands.UpdateJob;

[Authorize(Roles = Roles.Employer)]
public record UpdateJobCommand : ICommand
{
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
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

public class UpdateJobCommandValidator : AbstractValidator<UpdateJobCommand>
{
    public UpdateJobCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.JobId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255);
        RuleFor(x => x.SpecialtyId).NotEmpty();
        RuleFor(x => x.LocationId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}

public class UpdateJobCommandHandler : ICommandHandler<UpdateJobCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Unit> Handle(UpdateJobCommand command, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        if (!job.CanEdit)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.JobId), "Chỉ sửa được tin ở trạng thái draft hoặc bị từ chối.")]);
        }

        job.Title = command.Title;
        job.SpecialtyId = command.SpecialtyId;
        job.EmploymentType = command.EmploymentType;
        job.SalaryMin = command.SalaryMin;
        job.SalaryMax = command.SalaryMax;
        job.SalaryNegotiable = command.SalaryNegotiable;
        job.LocationId = command.LocationId;
        job.AddressDetail = command.AddressDetail;
        job.RequiredLicense = command.RequiredLicense;
        job.MinExperienceYears = command.MinExperienceYears;
        job.Description = command.Description;
        job.Requirements = command.Requirements;
        job.Benefits = command.Benefits;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
