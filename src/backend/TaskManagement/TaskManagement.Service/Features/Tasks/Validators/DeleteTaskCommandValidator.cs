using FluentValidation;
using TaskManagement.Service.Features.Tasks.Commands;

namespace TaskManagement.Service.Features.Tasks.Validators;

/// <summary>
/// Validator for DeleteTaskCommand.
/// </summary>
public class DeleteTaskCommandValidator : AbstractValidator<DeleteTaskCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteTaskCommandValidator"/> class.
    /// </summary>
    public DeleteTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Task ID is required.");
    }
}

