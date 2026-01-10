namespace Span.Culturio.Shared.Models.Entities
{
    public class Visit
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int CultureObjectId { get; set; }
        public DateTime VisitDate { get; set; }

        public Subscription Subscription { get; set; }
        public CultureObject CultureObject { get; set; }
    }
}
