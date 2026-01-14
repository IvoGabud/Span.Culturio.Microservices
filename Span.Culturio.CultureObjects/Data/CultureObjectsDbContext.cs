using Microsoft.EntityFrameworkCore;
using Span.Culturio.CultureObjects.Models.Entities;

namespace Span.Culturio.CultureObjects.Data
{
    public class CultureObjectsDbContext : DbContext
    {
        public CultureObjectsDbContext(DbContextOptions<CultureObjectsDbContext> options) : base(options)
        {
        }

        public DbSet<CultureObject> CultureObjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CultureObject>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.ContactEmail).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Address).HasMaxLength(250).IsRequired();
                entity.Property(e => e.City).HasMaxLength(250).IsRequired();

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
        }
    }
}
