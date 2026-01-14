using FluentValidation;
using Span.Culturio.CultureObjects.Models.DTOs;

namespace Span.Culturio.CultureObjects.Validators
{
    public class CreateCultureObjectDtoValidator : AbstractValidator<CreateCultureObjectDto>
    {
        public CreateCultureObjectDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

            RuleFor(x => x.ContactEmail)
                .NotEmpty().WithMessage("Contact email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(255).WithMessage("Contact email must not exceed 255 characters");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(250).WithMessage("Address must not exceed 250 characters");

            RuleFor(x => x.ZipCode)
                .GreaterThan(0).WithMessage("Zip code must be greater than 0")
                .InclusiveBetween(10000, 99999).WithMessage("Zip code must be a valid Croatian postal code (10000-99999)");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required")
                .MaximumLength(250).WithMessage("City must not exceed 250 characters");

            RuleFor(x => x.AdminUserId)
                .GreaterThan(0).WithMessage("Admin user ID must be greater than 0");
        }
    }
}
