using FluentValidation;

namespace BlogSystem.Application.Features.Blogs.Commands;

public class CreateBlogCommandValidator : AbstractValidator<CreateBlogCommand>
{
    public CreateBlogCommandValidator()
    {
        RuleFor(x => x.BlogDto.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.BlogDto.Body)
            .NotEmpty().WithMessage("Body is required")
            .MinimumLength(10).WithMessage("Body must be at least 10 characters");

        RuleFor(x => x.BlogDto.AuthorId)
            .NotEmpty().WithMessage("Author ID is required");
    }
}
