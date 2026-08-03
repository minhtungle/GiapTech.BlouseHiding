using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Jobs.Commands.RenewJob;

// Gia hạn LUÔN tạo jobs row mới (ERD mục 4.7) — không tái sử dụng job_id cũ, để không chặn ứng viên
// từng bị từ chối ứng tuyển lại ở đợt tuyển mới (UNIQUE(job_id, candidate_id) là job_id khác).
[Authorize(Roles = Roles.Employer)]
public record RenewJobCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
}

public class RenewJobCommandValidator : AbstractValidator<RenewJobCommand>
{
    public RenewJobCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.JobId).NotEmpty();
    }
}

public class RenewJobCommandHandler : ICommandHandler<RenewJobCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public RenewJobCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(RenewJobCommand command, CancellationToken cancellationToken)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == job.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var renewed = job.CreateRenewalCopy();

        _context.Jobs.Add(renewed);
        await _context.SaveChangesAsync(cancellationToken);

        return renewed.Id;
    }
}
