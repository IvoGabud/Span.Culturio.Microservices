namespace Span.Culturio.Users.Models.DTOs
{
    public class GetUsersQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
