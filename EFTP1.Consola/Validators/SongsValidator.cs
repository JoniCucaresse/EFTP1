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
            RuleFor(b => b.Title)
                .NotEmpty().WithMessage("The {PropertyName} is required")
                .MaximumLength(300).WithMessage("The {PropertyName} must have no more than {ComparisonValue} characters");

            RuleFor(b => b.Duration)
                .NotEmpty().WithMessage("The {PropertyName} is required")
                .GreaterThan(0).WithMessage("The {PropertyName} must be greather than {ComparisonValue}");

            RuleFor(b => b.Gender)
               .NotEmpty().WithMessage("The {PropertyName} is required")
               .Matches(@"^[a-zA-Z\s]+$").WithMessage("The {PropertyName} must contain only letters and spaces")
               .MaximumLength(300).WithMessage("The {PropertyName} must have no more than {ComparisonValue} characters");

            When(s => s.ArtistId == 0, () =>
            {
                RuleFor(s => s.ArtistId).Equal(0).WithMessage("When adding a new Author, AuthorId must be {ComparisonValue}");
            }).Otherwise(() => {

                RuleFor(s => s.ArtistId).GreaterThan(0).WithMessage("The field {PropertyName} must be greater than {ComparisonValue}");
            });

        }
    }
}
