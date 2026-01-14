using FluentValidation;
using Span.Culturio.Subscriptions.Models.DTOs;

namespace Span.Culturio.Subscriptions.Validators
{
    public class CreateSubscriptionDtoValidator : AbstractValidator<CreateSubscriptionDto>
    {
        public CreateSubscriptionDtoValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User ID must be greater than 0");

            RuleFor(x => x.PackageId)
                .GreaterThan(0).WithMessage("Package ID must be greater than 0");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Subscription name is required")
                .MaximumLength(100).WithMessage("Subscription name must not exceed 100 characters");
        }
    }
}
