using EFTP1.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFTP1.Consola.Validators
{
    public class ArtistValidator:AbstractValidator<Artist>
    {
        public ArtistValidator()
        {
            RuleFor(a => a.Name)
                 .NotEmpty().WithMessage("The field Name is required.")
                 .Matches(@"^[a-zA-Z\s]+$").WithMessage("The field Name must contain only letters and spaces.")
                 .Length(3, 50).WithMessage("The field Name must be between 3 and 50 characters.");

            RuleFor(a => a.Country)
                .NotEmpty().WithMessage("The field Country is required.")
                .Matches(@"^[a-zA-Z\s]+$").WithMessage("The field Country must contain only letters and spaces.")
                .Length(3, 50).WithMessage("The field Country must be between 3 and 50 characters.");

            RuleFor(a => a.FoundationYear)
                .NotEmpty().WithMessage("The {PropertyName} is required")
                .InclusiveBetween(1800, DateTime.Now.Year).WithMessage("The {PropertyName} must be between {From} and {To}");
        }
    }
   
}
