namespace Span.Culturio.Shared.Models.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ValidDays { get; set; }

        public ICollection<PackageCultureObject> PackageCultureObjects { get; set; }
        public ICollection<Subscription> Subscriptions { get; set; }
    }
}
