using FluentValidation;
using Span.Culturio.Subscriptions.Models.DTOs;

namespace Span.Culturio.Subscriptions.Validators
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
