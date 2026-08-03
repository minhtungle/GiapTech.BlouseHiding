using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Applications.Commands.SubmitApplication;

// Ứng viên nộp đơn — hồ sơ chưa xác thực CCHN vẫn ứng tuyển được (ERD mục 4.2, không chặn cứng), backend
// tự chụp cv_snapshot + tính score tại thời điểm này (không đổi khi hồ sơ gốc thay đổi sau — ERD mục 4.8).
[Authorize(Roles = Roles.Candidate)]
public record SubmitApplicationCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public Guid JobId { get; init; }
    public string? CoverLetter { get; init; }
}

public class SubmitApplicationCommandValidator : AbstractValidator<SubmitApplicationCommand>
{
    public SubmitApplicationCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.JobId).NotEmpty();
    }
}

public class SubmitApplicationCommandHandler : ICommandHandler<SubmitApplicationCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public SubmitApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(SubmitApplicationCommand command, CancellationToken cancellationToken)
    {
        var profile = await _context.CandidateProfiles
            .Include(p => p.Specialties)
            .Include(p => p.Licenses)
            .FirstOrDefaultAsync(p => p.UserId == command.UserId, cancellationToken)
            ?? throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.UserId), "Cần hoàn thiện hồ sơ trước khi ứng tuyển.")]);

        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == command.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Job), command.JobId);

        if (job.Status != JobStatus.Published)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.JobId), "Chỉ ứng tuyển được vào tin đang published.")]);
        }

        var alreadyApplied = await _context.Applications
            .AnyAsync(a => a.JobId == command.JobId && a.CandidateId == profile.Id, cancellationToken);

        if (alreadyApplied)
        {
            throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                nameof(command.JobId), "Bạn đã ứng tuyển vào tin này rồi.")]);
        }

        var application = new Domain.Entities.JobApplication
        {
            JobId = command.JobId,
            CandidateId = profile.Id,
            CoverLetter = command.CoverLetter,
            CvSnapshot = CvSnapshotBuilder.Build(profile),
            Score = ApplicationScoring.Calculate(profile, job),
            AppliedAt = DateTimeOffset.UtcNow,
        };

        _context.Applications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);

        return application.Id;
    }
}
