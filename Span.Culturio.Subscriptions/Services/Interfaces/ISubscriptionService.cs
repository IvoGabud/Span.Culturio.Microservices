using Span.Culturio.Shared.Models.DTOs;
using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.Subscriptions.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateAsync(CreateSubscriptionDto dto);
        Task<List<Subscription>> GetSubscriptionsAsync(int? userId);
        Task<bool> TrackVisitAsync(TrackVisitDto dto);
        Task<bool> ActivateSubscriptionAsync(ActivateSubscriptionDto dto);
    }
}
