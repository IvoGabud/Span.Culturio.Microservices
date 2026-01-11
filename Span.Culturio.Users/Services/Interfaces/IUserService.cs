using Span.Culturio.Shared.Models.DTOs;

namespace Span.Culturio.Users.Services.Interfaces
{
    public interface IUserService
    {
        Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersAsync(int page, int pageSize);
        Task<UserDto?> GetUserByIdAsync(int id);
    }
}
