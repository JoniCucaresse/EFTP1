using EFTP1.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTP1.Consola.Validators
{
    public class SongsValidator: AbstractValidator<Song>
    {
        public SongsValidator()
        {
            RuleFor(b => b.Title).NotEmpty().WithMessage("The {PropertyName} is required")
                .MaximumLength(300).WithMessage("The {PropertyName} must have no more than {ComparisonValue} characters");

            RuleFor(b => b.Duration).NotEmpty().WithMessage("The {PropertyName} is required")
                .GreaterThan(0).WithMessage("The {PropertyName} must be greather than {ComparisonValue}");

            RuleFor(b => b.Gender).NotEmpty().WithMessage("The {PropertyName} is required")
               .MaximumLength(300).WithMessage("The {PropertyName} must have no more than {ComparisonValue} characters");

        }
    }
}
