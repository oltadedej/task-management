using FluentValidation;
using TaskManagement.Service.Features.Tasks.Queries;

namespace TaskManagement.Service.Features.Tasks.Validators;

/// <summary>
/// Validator for GetTaskByIdQuery.
/// </summary>
public class GetTaskByIdQueryValidator : AbstractValidator<GetTaskByIdQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetTaskByIdQueryValidator"/> class.
    /// </summary>
    public GetTaskByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Task ID is required.");
    }
}
