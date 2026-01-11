using Microsoft.EntityFrameworkCore;
using Span.Culturio.Shared.Models.Entities;

namespace Span.Culturio.Packages.Data
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

                entity.HasData(
                    new User
                    {
                        Id = 1,
                        FirstName = "Admin",
                        LastName = "User",
                        Email = "admin@culturio.hr",
                        Username = "admin",
                        PasswordHash = "$2a$11$tz4JCwuUMuWtSelRA.C5NOISqpWL5E4vBEYaX903GzpMscp2qeCVi",
                        Role = "Admin"
                    }
                );
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

                entity.HasData(
                    new CultureObject
                    {
                        Id = 1,
                        Name = "Muzej Mimara",
                        ContactEmail = "kontakt@mimara.hr",
                        Address = "Rooseveltov trg 5",
                        ZipCode = 10000,
                        City = "Zagreb",
                        AdminUserId = 1
                    },
                    new CultureObject
                    {
                        Id = 2,
                        Name = "Hrvatsko narodno kazalište",
                        ContactEmail = "info@hnk.hr",
                        Address = "Trg Republike Hrvatske 15",
                        ZipCode = 10000,
                        City = "Zagreb",
                        AdminUserId = 1
                    },
                    new CultureObject
                    {
                        Id = 3,
                        Name = "Muzej suvremene umjetnosti",
                        ContactEmail = "kontakt@msu.hr",
                        Address = "Avenija Dubrovnik 17",
                        ZipCode = 10000,
                        City = "Zagreb",
                        AdminUserId = 1
                    }
                );
            });

            modelBuilder.Entity<Package>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();

                entity.HasData(
                    new Package { Id = 1, Name = "Osnovni paket", ValidDays = 30 },
                    new Package { Id = 2, Name = "Premium paket", ValidDays = 90 },
                    new Package { Id = 3, Name = "Godišnji paket", ValidDays = 365 }
                );
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

                entity.HasData(
                    new PackageCultureObject { Id = 1, PackageId = 1, CultureObjectId = 1, AvailableVisits = 5 },
                    new PackageCultureObject { Id = 2, PackageId = 1, CultureObjectId = 2, AvailableVisits = 3 },
                    new PackageCultureObject { Id = 3, PackageId = 1, CultureObjectId = 3, AvailableVisits = 2 },
                    new PackageCultureObject { Id = 4, PackageId = 2, CultureObjectId = 1, AvailableVisits = 10 },
                    new PackageCultureObject { Id = 5, PackageId = 2, CultureObjectId = 2, AvailableVisits = 8 },
                    new PackageCultureObject { Id = 6, PackageId = 2, CultureObjectId = 3, AvailableVisits = 6 },
                    new PackageCultureObject { Id = 7, PackageId = 3, CultureObjectId = 1, AvailableVisits = 30 },
                    new PackageCultureObject { Id = 8, PackageId = 3, CultureObjectId = 2, AvailableVisits = 25 },
                    new PackageCultureObject { Id = 9, PackageId = 3, CultureObjectId = 3, AvailableVisits = 20 }
                );
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
