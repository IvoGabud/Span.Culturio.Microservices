using Span.Culturio.Subscriptions.Models.DTOs;
using Span.Culturio.Subscriptions.Models.Entities;

namespace Span.Culturio.Subscriptions.Services.Interfaces
{
    public interface ISubscriptionService
    {
        Task<Subscription> CreateAsync(CreateSubscriptionDto dto);
        Task<List<Subscription>> GetSubscriptionsAsync(int? userId);
        Task<(bool Success, string? ErrorMessage)> TrackVisitAsync(TrackVisitDto dto);
        Task<bool> ActivateSubscriptionAsync(ActivateSubscriptionDto dto);
    }
}
