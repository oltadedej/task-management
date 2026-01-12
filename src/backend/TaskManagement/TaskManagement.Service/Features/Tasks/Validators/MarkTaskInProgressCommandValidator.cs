using FluentValidation;
using TaskManagement.Service.Features.Tasks.Commands;

namespace TaskManagement.Service.Features.Tasks.Validators;

/// <summary>
/// Validator for MarkTaskInProgressCommand.
/// </summary>
public class MarkTaskInProgressCommandValidator : AbstractValidator<MarkTaskInProgressCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MarkTaskInProgressCommandValidator"/> class.
    /// </summary>
    public MarkTaskInProgressCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Task ID is required.");
    }
}

