using FluentValidation;
using Span.Culturio.Subscriptions.Models.DTOs;

namespace Span.Culturio.Subscriptions.Validators
{
    public class TrackVisitDtoValidator : AbstractValidator<TrackVisitDto>
    {
        public TrackVisitDtoValidator()
        {
            RuleFor(x => x.SubscriptionId)
                .GreaterThan(0).WithMessage("Subscription ID must be greater than 0");

            RuleFor(x => x.CultureObjectId)
                .GreaterThan(0).WithMessage("Culture object ID must be greater than 0");
        }
    }
}
