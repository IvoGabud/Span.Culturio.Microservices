using FluentValidation;
using Span.Culturio.Shared.Models.DTOs;

namespace Span.Culturio.Shared.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MaximumLength(100).WithMessage("Username must not exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MaximumLength(255).WithMessage("Password must not exceed 255 characters");
        }
    }
}
