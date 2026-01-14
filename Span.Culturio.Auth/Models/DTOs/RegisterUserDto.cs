using System.ComponentModel.DataAnnotations;

namespace Span.Culturio.Auth.Models.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(100)]
        [Required]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(255)]
        [EmailAddress]
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        [MaxLength(255)]
        [Required]
        public string Password { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? Role { get; set; }
    }
}
