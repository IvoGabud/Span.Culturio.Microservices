using System.ComponentModel.DataAnnotations;

namespace Span.Culturio.Auth.Models.DTOs
{
    public class LoginDto
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;
    }
}
