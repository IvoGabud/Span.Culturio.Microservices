using Microsoft.EntityFrameworkCore;
using Span.Culturio.Auth.Models.Entities;

namespace Span.Culturio.Auth.Data
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

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
        }
    }
}
