using System.ComponentModel.DataAnnotations;

namespace Span.Culturio.Shared.Models.DTOs
{
    public class ActivateSubscriptionDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int SubscriptionId { get; set; }
    }
}
