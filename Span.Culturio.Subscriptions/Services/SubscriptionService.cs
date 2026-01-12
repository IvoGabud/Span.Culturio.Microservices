using Microsoft.EntityFrameworkCore;
using Span.Culturio.Subscriptions.Data;
using Span.Culturio.Subscriptions.Services.Interfaces;
using Span.Culturio.Shared.Models.DTOs;
using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.Subscriptions.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly CulturioDbContext _context;
        private readonly ILogger<SubscriptionService> _logger;

        public SubscriptionService(CulturioDbContext context, ILogger<SubscriptionService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Subscription> CreateAsync(CreateSubscriptionDto dto)
        {
            _logger.LogInformation("Creating subscription for User {UserId}, Package {PackageId}", dto.UserId, dto.PackageId);

            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.UserId);
            if (!userExists)
            {
                _logger.LogWarning("User with ID {UserId} does not exist", dto.UserId);
                throw new InvalidOperationException($"User with ID {dto.UserId} does not exist");
            }

            var packageExists = await _context.Packages.AnyAsync(p => p.Id == dto.PackageId);
            if (!packageExists)
            {
                _logger.LogWarning("Package with ID {PackageId} does not exist", dto.PackageId);
                throw new InvalidOperationException($"Package with ID {dto.PackageId} does not exist");
            }

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

            var query = _context.Subscriptions
                .Include(s => s.Package)
                .AsQueryable();

            if (userId.HasValue)
                query = query.Where(s => s.UserId == userId.Value);

            var subscriptions = await query.ToListAsync();

            _logger.LogInformation("Fetched {Count} subscriptions", subscriptions.Count);

            return subscriptions;
        }

        public async Task<bool> TrackVisitAsync(TrackVisitDto dto)
        {
            _logger.LogInformation("Tracking visit for Subscription {SubscriptionId}, CultureObject {CultureObjectId}",
                dto.SubscriptionId, dto.CultureObjectId);

            var subscription = await _context.Subscriptions
                .Include(s => s.Package)
                    .ThenInclude(p => p.PackageCultureObjects)
                .FirstOrDefaultAsync(s => s.Id == dto.SubscriptionId);

            if (subscription == null || subscription.State != "active")
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found or not active", dto.SubscriptionId);
                return false;
            }

            if (subscription.ActiveTo.HasValue && subscription.ActiveTo.Value < DateTime.UtcNow)
            {
                _logger.LogInformation("Subscription {SubscriptionId} has expired, updating state", dto.SubscriptionId);
                subscription.State = "expired";
                await _context.SaveChangesAsync();
                return false;
            }

            var packageCultureObject = subscription.Package.PackageCultureObjects
                .FirstOrDefault(pco => pco.CultureObjectId == dto.CultureObjectId);

            if (packageCultureObject == null)
            {
                _logger.LogWarning("CultureObject {CultureObjectId} not found in package", dto.CultureObjectId);
                return false;
            }

            var visitsToThisCultureObject = await _context.Visits
                .CountAsync(v => v.SubscriptionId == dto.SubscriptionId
                    && v.CultureObjectId == dto.CultureObjectId);

            if (visitsToThisCultureObject >= packageCultureObject.AvailableVisits)
            {
                _logger.LogWarning("Visit limit reached for CultureObject {CultureObjectId} ({Visits}/{Limit})",
                    dto.CultureObjectId, visitsToThisCultureObject, packageCultureObject.AvailableVisits);
                return false;
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

            return true;
        }

        public async Task<bool> ActivateSubscriptionAsync(ActivateSubscriptionDto dto)
        {
            _logger.LogInformation("Activating subscription {SubscriptionId}", dto.SubscriptionId);

            var subscription = await _context.Subscriptions
                .Include(s => s.Package)
                .FirstOrDefaultAsync(s => s.Id == dto.SubscriptionId);

            if (subscription == null)
            {
                _logger.LogWarning("Subscription {SubscriptionId} not found", dto.SubscriptionId);
                return false;
            }

            subscription.State = "active";
            subscription.ActiveFrom = DateTime.UtcNow;
            subscription.ActiveTo = DateTime.UtcNow.AddDays(subscription.Package.ValidDays);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Subscription {SubscriptionId} activated until {ActiveTo}",
                dto.SubscriptionId, subscription.ActiveTo);

            return true;
        }
    }
}
