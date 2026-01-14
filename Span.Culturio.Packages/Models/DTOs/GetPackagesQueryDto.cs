namespace Span.Culturio.Packages.Models.DTOs
{
    public class GetPackagesQueryDto
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
