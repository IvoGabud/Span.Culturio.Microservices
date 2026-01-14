using Microsoft.EntityFrameworkCore;
using Span.Culturio.Subscriptions.Models.Entities;

namespace Span.Culturio.Subscriptions.Data
{
    public class SubscriptionsDbContext : DbContext
    {
        public SubscriptionsDbContext(DbContextOptions<SubscriptionsDbContext> options) : base(options)
        {
        }

        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Visit> Visits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.State).HasMaxLength(50).IsRequired();
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Subscription)
                    .WithMany(s => s.Visits)
                    .HasForeignKey(e => e.SubscriptionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
