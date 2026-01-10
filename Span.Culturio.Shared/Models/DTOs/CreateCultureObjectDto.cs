using System.ComponentModel.DataAnnotations;

namespace Span.Culturio.Shared.Models.DTOs
{
    public class CreateCultureObjectDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ContactEmail { get; set; } = string.Empty;

        [Required]
        [MaxLength(250)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int ZipCode { get; set; }

        [Required]
        [MaxLength(250)]
        public string City { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int AdminUserId { get; set; }
    }
}
