using FluentValidation;

namespace BlogSystem.Application.Features.Authors.Commands;

public class CreateAuthorCommandValidator : AbstractValidator<CreateAuthorCommand>
{
    public CreateAuthorCommandValidator()
    {
        RuleFor(command => command.AuthorDto)
            .NotNull().WithMessage("Author data is required.");

        When(command => command.AuthorDto != null, () =>
        {
            RuleFor(command => command.AuthorDto.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(command => command.AuthorDto.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.");

            RuleFor(command => command.AuthorDto.ImageUrl)
                .MaximumLength(500).WithMessage("Image Url cannot exceed 500 characters.")
                .Matches((@"^(https?://)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$"))
                    .WithMessage("A valid URL is required for ImageUrl.")
                 .When(command => !string.IsNullOrEmpty(command.AuthorDto.ImageUrl));
                    
        });
    }
}
