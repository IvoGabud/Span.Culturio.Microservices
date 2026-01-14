using Microsoft.EntityFrameworkCore;
using Span.Culturio.Packages.Models.Entities;

namespace Span.Culturio.Packages.Data
{
    public class PackagesDbContext : DbContext
    {
        public PackagesDbContext(DbContextOptions<PackagesDbContext> options) : base(options)
        {
        }

        public DbSet<Package> Packages { get; set; }
        public DbSet<PackageCultureObject> PackageCultureObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }
    }
}
