using BulletinBoard.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BulletinBoard.DAL
{
    public class BulletinBoardContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Ad> Ads { get; set; }

        public BulletinBoardContext(DbContextOptions<BulletinBoardContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Налаштування зовнішніх ключів і відносин
            modelBuilder.Entity<Ad>()
                .HasOne(a => a.User)
                .WithMany(u => u.Ads)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ad>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Ads)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.SetNull); // Встановлення для CategoryId: якщо категорія буде видалена, оголошення залишаться
        }
    }
}
