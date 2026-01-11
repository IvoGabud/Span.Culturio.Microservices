using Microsoft.EntityFrameworkCore;
using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.Users.Data
{
    public class CulturioDbContext : DbContext
    {
        public CulturioDbContext(DbContextOptions<CulturioDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<CultureObject> CultureObjects { get; set; }
        public DbSet<PackageCultureObject> PackageCultureObjects { get; set; }
        public DbSet<Visit> Visits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Username).HasMaxLength(100).IsRequired();
                entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired().HasDefaultValue("User");
            });

            modelBuilder.Entity<CultureObject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ContactEmail).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(250).IsRequired();
                entity.Property(e => e.City).HasMaxLength(250).IsRequired();

                entity.HasOne(e => e.AdminUser)
                    .WithMany()
                    .HasForeignKey(e => e.AdminUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<PackageCultureObject>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Package)
                    .WithMany(p => p.PackageCultureObjects)
                    .HasForeignKey(e => e.PackageId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CultureObject)
                    .WithMany(c => c.PackageCultureObjects)
                    .HasForeignKey(e => e.CultureObjectId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Subscription>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.State).HasMaxLength(50).IsRequired();

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Subscriptions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Package)
                    .WithMany(p => p.Subscriptions)
                    .HasForeignKey(e => e.PackageId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Visit>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Subscription)
                    .WithMany(s => s.Visits)
                    .HasForeignKey(e => e.SubscriptionId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.CultureObject)
                    .WithMany()
                    .HasForeignKey(e => e.CultureObjectId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
