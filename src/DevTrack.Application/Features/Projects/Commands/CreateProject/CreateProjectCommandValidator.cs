using FluentValidation;

namespace DevTrack.Application.Features.Projects.Commands.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Key)
            .NotEmpty().WithMessage("Project key is required")
            .MinimumLength(2).WithMessage("Project key must be at least 2 characters")
            .MaximumLength(10).WithMessage("Project key must not exceed 10 characters")
            .Matches("^[A-Z][A-Z0-9]*$").WithMessage("Project key must start with a letter and contain only uppercase letters and numbers");
    }
}