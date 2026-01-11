using Microsoft.EntityFrameworkCore;
using Span.Culturio.Shared.Models.DTOs;
using Span.Culturio.Users.Data;
using Span.Culturio.Users.Services.Interfaces;

namespace Span.Culturio.Users.Services
{
    public class UserService : IUserService
    {
        private readonly CulturioDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(CulturioDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<(IEnumerable<UserDto> Users, int TotalCount)> GetUsersAsync(int page, int pageSize)
        {
            _logger.LogInformation("Fetching users - Page: {Page}, PageSize: {PageSize}", page, pageSize);

            var totalCount = await _context.Users.CountAsync();

            var users = await _context.Users
                .OrderBy(u => u.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Username = u.Username,
                    Role = u.Role
                })
                .ToListAsync();

            _logger.LogInformation("Fetched {Count} users out of {TotalCount}", users.Count, totalCount);

            return (users, totalCount);
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            _logger.LogInformation("Fetching user by ID: {UserId}", id);

            var user = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Username = u.Username,
                    Role = u.Role
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
            }

            return user;
        }
    }
}
