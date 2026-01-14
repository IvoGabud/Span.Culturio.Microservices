using Microsoft.EntityFrameworkCore;
using Span.Culturio.Subscriptions.Data;
using Span.Culturio.Subscriptions.Services.Interfaces;
using Span.Culturio.Subscriptions.Models.DTOs;
using Span.Culturio.Subscriptions.Models.Entities;

namespace Span.Culturio.Subscriptions.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly SubscriptionsDbContext _context;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(SubscriptionsDbContext context, ILogger<SubscriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Subscription> CreateAsync(CreateSubscriptionDto dto)
        {
            _logger.LogInformation("Creating subscription for User {UserId}, Package {PackageId}", dto.UserId, dto.PackageId);

            var subscription = new Subscription
            {
                UserId = dto.UserId,
                PackageId = dto.PackageId,
                Name = dto.Name,
                State = "expired",
                RecordedVisits = 0,
                ActiveFrom = null,
                ActiveTo = null
            };

            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription created with ID: {SubscriptionId}", subscription.Id);

            return subscription;
        }

        public async Task<List<Subscription>> GetSubscriptionsAsync(int? userId)
        {
            _logger.LogInformation("Fetching subscriptions" + (userId.HasValue ? $" for User {userId.Value}" : ""));

            var query = _context.Subscriptions.AsQueryable();

            if (userId.HasValue)
                query = query.Where(s => s.UserId == userId.Value);

            var subscriptions = await query.ToListAsync();

            _logger.LogInformation("Fetched {Count} subscriptions", subscriptions.Count);

            return subscriptions;
        }

        public async Task<(bool Success, string? ErrorMessage)> TrackVisitAsync(TrackVisitDto dto)
        {
            _logger.LogInformation("Tracking visit for Subscription {SubscriptionId}, CultureObject {CultureObjectId}",
                dto.SubscriptionId, dto.CultureObjectId);

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Id == dto.SubscriptionId);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", dto.SubscriptionId);
                return (false, "Subscription not found");
            }

            if (subscription.State != "active")
            {
                _logger.LogWarning("Subscription {SubscriptionId} is not activated", dto.SubscriptionId);
                return (false, "Subscription is not activated");
            }

            if (subscription.ActiveTo.HasValue && subscription.ActiveTo.Value < DateTime.UtcNow)
            {
                _logger.LogInformation("Subscription {SubscriptionId} has expired, updating state", dto.SubscriptionId);
                subscription.State = "expired";
                await _context.SaveChangesAsync();
                return (false, "Subscription has expired");
            }

            var visit = new Visit
            {
                SubscriptionId = dto.SubscriptionId,
                CultureObjectId = dto.CultureObjectId,
                VisitDate = DateTime.UtcNow
            };

            _context.Visits.Add(visit);
            subscription.RecordedVisits++;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Visit tracked successfully. Total visits: {TotalVisits}", subscription.RecordedVisits);

            return (true, null);
        }

        public async Task<bool> ActivateSubscriptionAsync(ActivateSubscriptionDto dto)
        {
            _logger.LogInformation("Activating subscription {SubscriptionId}", dto.SubscriptionId);

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(s => s.Id == dto.SubscriptionId);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", dto.SubscriptionId);
                return false;
            }

            subscription.State = "active";
            subscription.ActiveFrom = DateTime.UtcNow;
            subscription.ActiveTo = DateTime.UtcNow.AddDays(30);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription {SubscriptionId} activated until {ActiveTo}",
                dto.SubscriptionId, subscription.ActiveTo);

            return true;
        }
    }
}
