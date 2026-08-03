using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using GiapTech.BlouseHiding.Domain.Enums;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;
using ValidationException = GiapTech.BlouseHiding.Application.Common.Exceptions.ValidationException;

namespace GiapTech.BlouseHiding.Application.Employers.Commands.UnlockProfile;

// Trừ Credit qua UPDATE có điều kiện (WHERE Balance >= cost) — atomic ngay tại DB, tránh race
// condition khi 2 request trừ đồng thời mà không cần đọc-sửa-ghi qua ChangeTracker (CLAUDE.md mục 4
// quy tắc bất di bất dịch #2).
[Authorize(Roles = Roles.Employer)]
public record UnlockProfileCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public Guid OrganizationId { get; init; }
    public Guid CandidateId { get; init; }
}

public class UnlockProfileCommandValidator : AbstractValidator<UnlockProfileCommand>
{
    public UnlockProfileCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrganizationId).NotEmpty();
        RuleFor(x => x.CandidateId).NotEmpty();
    }
}

public class UnlockProfileCommandHandler : ICommandHandler<UnlockProfileCommand, Guid>
{
    private const int UnlockCost = 15;

    private readonly IApplicationDbContext _context;

    public UnlockProfileCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(UnlockProfileCommand command, CancellationToken cancellationToken)
    {
        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == command.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var candidateExists = await _context.CandidateProfiles.AnyAsync(p => p.Id == command.CandidateId, cancellationToken);
        if (!candidateExists)
        {
            throw new NotFoundException(nameof(Domain.Entities.CandidateProfile), command.CandidateId);
        }

        var existingUnlock = await _context.ProfileUnlocks
            .FirstOrDefaultAsync(u => u.OrganizationId == command.OrganizationId && u.CandidateId == command.CandidateId, cancellationToken);

        if (existingUnlock is not null)
        {
            // Idempotent — mở 1 lần, xem lại không mất thêm credit (ERD mục 2.7).
            return existingUnlock.Id;
        }

        var wallet = await _context.CreditWallets
            .FirstOrDefaultAsync(w => w.OrganizationId == command.OrganizationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.CreditWallet), command.OrganizationId);

        return await _context.ExecuteInTransactionAsync(async () =>
        {
            var rowsAffected = await _context.CreditWallets
                .Where(w => w.Id == wallet.Id && w.Balance >= UnlockCost)
                .ExecuteUpdateAsync(setters => setters.SetProperty(w => w.Balance, w => w.Balance - UnlockCost), cancellationToken);

            if (rowsAffected == 0)
            {
                throw new ValidationException([new FluentValidation.Results.ValidationFailure(
                    nameof(command.OrganizationId), "Số dư Credit không đủ để mở hồ sơ này.")]);
            }

            var unlock = new Domain.Entities.ProfileUnlock
            {
                OrganizationId = command.OrganizationId,
                CandidateId = command.CandidateId,
                CreditCost = UnlockCost,
                UnlockedBy = command.UserId,
            };
            _context.ProfileUnlocks.Add(unlock);

            _context.CreditTransactions.Add(new Domain.Entities.CreditTransaction
            {
                WalletId = wallet.Id,
                Amount = -UnlockCost,
                Reason = CreditTransactionReason.UnlockProfile,
                ReferenceId = unlock.Id,
                CreatedBy = null,
            });

            await _context.SaveChangesAsync(cancellationToken);

            return unlock.Id;
        }, cancellationToken);
    }
}
