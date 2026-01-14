namespace Span.Culturio.Subscriptions.Models.Entities
{
    public class Subscription
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PackageId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime? ActiveFrom { get; set; }
        public DateTime? ActiveTo { get; set; }
        public string State { get; set; } = "expired";
        public int RecordedVisits { get; set; }

        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}
