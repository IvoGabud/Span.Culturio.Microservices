using FluentValidation;
using Span.Culturio.Shared.Models.DTOs;

namespace Span.Culturio.Shared.Validators
{
    public class ActivateSubscriptionDtoValidator : AbstractValidator<ActivateSubscriptionDto>
    {
        public ActivateSubscriptionDtoValidator()
        {
            RuleFor(x => x.SubscriptionId)
                .GreaterThan(0).WithMessage("Subscription ID must be greater than 0");
        }
    }
}
