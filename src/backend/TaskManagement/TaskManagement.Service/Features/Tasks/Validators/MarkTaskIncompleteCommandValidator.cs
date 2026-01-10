using FluentValidation;
using TaskManagement.Service.Features.Tasks.Commands;

namespace TaskManagement.Service.Features.Tasks.Validators;

/// <summary>
/// Validator for MarkTaskIncompleteCommand.
/// </summary>
public class MarkTaskIncompleteCommandValidator : AbstractValidator<MarkTaskIncompleteCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkTaskIncompleteCommandValidator"/> class.
    /// </summary>
    public MarkTaskIncompleteCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Task ID is required.");
    }
}