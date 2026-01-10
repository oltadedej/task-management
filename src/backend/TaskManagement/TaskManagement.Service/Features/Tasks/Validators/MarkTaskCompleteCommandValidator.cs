using FluentValidation;
using TaskManagement.Service.Features.Tasks.Commands;

namespace TaskManagement.Service.Features.Tasks.Validators;

/// <summary>
/// Validator for MarkTaskCompleteCommand.
/// </summary>
public class MarkTaskCompleteCommandValidator : AbstractValidator<MarkTaskCompleteCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkTaskCompleteCommandValidator"/> class.
    /// </summary>
    public MarkTaskCompleteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Task ID is required.");
    }
}

