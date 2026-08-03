using GiapTech.BlouseHiding.Application.Common.Exceptions;
using GiapTech.BlouseHiding.Application.Common.Interfaces;
using GiapTech.BlouseHiding.Application.Common.Security;
using GiapTech.BlouseHiding.Domain.Constants;
using NotFoundException = GiapTech.BlouseHiding.Application.Common.Exceptions.NotFoundException;

namespace GiapTech.BlouseHiding.Application.Applications.Commands.AddApplicationNote;

[Authorize(Roles = Roles.Employer)]
public record AddApplicationNoteCommand : ICommand<Guid>
{
    public Guid UserId { get; init; }
    public Guid ApplicationId { get; init; }
    public string NoteText { get; init; } = string.Empty;
}

public class AddApplicationNoteCommandValidator : AbstractValidator<AddApplicationNoteCommand>
{
    public AddApplicationNoteCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ApplicationId).NotEmpty();
        RuleFor(x => x.NoteText).NotEmpty();
    }
}

public class AddApplicationNoteCommandHandler : ICommandHandler<AddApplicationNoteCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public AddApplicationNoteCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Guid> Handle(AddApplicationNoteCommand command, CancellationToken cancellationToken)
    {
        var application = await _context.Applications
            .Include(a => a.Job)
            .FirstOrDefaultAsync(a => a.Id == command.ApplicationId, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.JobApplication), command.ApplicationId);

        var isMember = await _context.EmployerMembers
            .AnyAsync(m => m.OrganizationId == application.Job!.OrganizationId && m.UserId == command.UserId, cancellationToken);

        if (!isMember)
        {
            throw new ForbiddenAccessException();
        }

        var note = new Domain.Entities.ApplicationNote
        {
            ApplicationId = command.ApplicationId,
            AuthorUserId = command.UserId,
            NoteText = command.NoteText,
        };

        _context.ApplicationNotes.Add(note);
        await _context.SaveChangesAsync(cancellationToken);

        return note.Id;
    }
}
