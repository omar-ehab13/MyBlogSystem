using FluentValidation;

namespace BlogSystem.Application.Features.Authors.Commands.DeleteAuthorCommand;

public class DeleteAuthorCommandValidator : AbstractValidator<DeleteAuthorCommand>
{
    public DeleteAuthorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Author ID cannot be empty.")
            .Must(id => id != Guid.Empty).WithMessage("Author ID must be a valid GUID and not Guid.Empty.");
    }
}
