using System.ComponentModel.DataAnnotations;

namespace Span.Culturio.Subscriptions.Models.DTOs
{
    public class CreateSubscriptionDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PackageId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
