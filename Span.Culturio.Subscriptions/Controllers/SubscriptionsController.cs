using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Span.Culturio.Shared.Models.DTOs;
using Span.Culturio.Subscriptions.Services.Interfaces;

namespace Span.Culturio.Subscriptions.Controllers
{
    [Route("subscriptions")]
    [ApiController]
    [Authorize]
    public class SubscriptionsController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger<SubscriptionsController> _logger;

        public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateSubscriptionDto dto)
        {
            _logger.LogInformation("POST /subscriptions - Creating subscription");

            try
            {
                var subscription = await _subscriptionService.CreateAsync(dto);
                return Ok(new { message = "Subscription created", id = subscription.Id });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Failed to create subscription: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubscriptions([FromQuery] int? userId)
        {
            _logger.LogInformation("GET /subscriptions - Fetching subscriptions");

            var subscriptions = await _subscriptionService.GetSubscriptionsAsync(userId);

            var result = subscriptions.Select(s => new
            {
                s.Id,
                s.UserId,
                s.PackageId,
                s.Name,
                s.ActiveFrom,
                s.ActiveTo,
                s.State,
                s.RecordedVisits
            });

            return Ok(result);
        }

        [HttpPost("track-visit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TrackVisit([FromBody] TrackVisitDto dto)
        {
            _logger.LogInformation("POST /subscriptions/track-visit - Tracking visit");

            var result = await _subscriptionService.TrackVisitAsync(dto);

            if (!result)
            {
                _logger.LogWarning("Unable to track visit");
                return BadRequest(new { message = "Unable to track visit" });
            }

            return Ok(new { message = "Visit tracked successfully" });
        }

        [HttpPost("activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ActivateSubscription([FromBody] ActivateSubscriptionDto dto)
        {
            _logger.LogInformation("POST /subscriptions/activate - Activating subscription");

            var result = await _subscriptionService.ActivateSubscriptionAsync(dto);

            if (!result)
            {
                _logger.LogWarning("Unable to activate subscription");
                return BadRequest(new { message = "Unable to activate subscription" });
            }

            return Ok(new { message = "Subscription activated successfully" });
        }
    }
}
