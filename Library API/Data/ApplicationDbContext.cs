
using Library_API.Model;
using Library_API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library_API.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookCopy> BookCopies { get; set; }
        public DbSet<User> LegacyUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Name = "The Great Gatsby",
                    Author = "F. Scott Fitzgerald",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Description = "A classic American novel",
                    Pages = 180,
                    Price = 12.99m,
                    PublishedYear = new DateTime(1925, 1, 1)
                },
                new Book
                {
                    Id = 2,
                    Name = "To Kill a Mockingbird",
                    Author = "Harper Lee",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Description = "A novel about racial injustice in the Deep South",
                    Pages = 281,
                    Price = 14.99m,
                    PublishedYear = new DateTime(1960, 7, 11)
                },
                new Book
                {
                    Id = 3,
                    Name = "1984",
                    Author = "George Orwell",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Description = "A dystopian novel about a totalitarian regime",
                    Pages = 328,
                    Price = 9.99m,
                    PublishedYear = new DateTime(1949, 6, 8)
                },
                new Book
                {
                    Id = 4,
                    Name = "Pride and Prejudice",
                    Author = "Jane Austen",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Description = "A romantic novel about the manners and customs of the British gentry",
                    Pages = 279,
                    Price = 11.99m,
                    PublishedYear = new DateTime(1813, 1, 28)
                }
            );

            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Name)
                .IsUnique();

            modelBuilder.Entity<User>()
                .ToTable("Users");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();
        }
    }
}
