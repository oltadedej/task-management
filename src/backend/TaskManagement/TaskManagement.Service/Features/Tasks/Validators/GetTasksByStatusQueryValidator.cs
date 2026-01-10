using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Service.Features.Tasks.Queries;

namespace TaskManagement.Service.Features.Tasks.Validators
{
    /// <summary>
    /// Validator for GetTasksByStatusQueryValidator.
    /// </summary>
    public class GetTasksByStatusQueryValidator : AbstractValidator<GetTasksByStatusQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetTasksByStatusQueryValidator"/> class.
        /// </summary>
        public GetTasksByStatusQueryValidator()
        {
            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Passed value is not part of Status Values");
        }
    }

}
