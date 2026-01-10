namespace Span.Culturio.Shared.Models.Entities
{
    public class CultureObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public int ZipCode { get; set; }
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public int AdminUserId { get; set; }

        public User AdminUser { get; set; }
        public ICollection<PackageCultureObject> PackageCultureObjects { get; set; }

    }
}
