using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Span.Culturio.Users.Models.DTOs;
using Span.Culturio.Users.Services.Interfaces;

namespace Span.Culturio.Users.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] GetUsersQueryDto query)
        {
            var (users, totalCount) = await _userService.GetUsersAsync(query.Page, query.PageSize);

            return Ok(new
            {
                Data = users,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { message = $"User with ID {id} not found" });
            }

            return Ok(user);
        }
    }
}
