using Span.Culturio.Shared.Models.DTOs;

namespace Span.Culturio.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterUserDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
