using System.ComponentModel.DataAnnotations;

namespace Span.Culturio.Subscriptions.Models.DTOs
{
    public class TrackVisitDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int SubscriptionId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CultureObjectId { get; set; }
    }
}
