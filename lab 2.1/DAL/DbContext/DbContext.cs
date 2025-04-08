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

            // Налаштування зовнішніх ключів та відносин для Ad
            modelBuilder.Entity<Ad>()
                .HasOne(a => a.User)
                .WithMany(u => u.Ads)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Ad>()
                .HasOne(a => a.Category)
                .WithMany(c => c.Ads)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Змінено на Restrict для попередження каскадного видалення

            // Налаштування відносин для Category з самою собою (ієрархія)
            modelBuilder.Entity<Category>()
                .HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Налаштування для багато-до-багатьох між Ads і Tags
            modelBuilder.Entity<Ad>()
                .HasMany(a => a.Tags)
                .WithMany(t => t.Ads)
                .UsingEntity(j => j
                    .ToTable("AdTags")
                    .HasKey("AdsId", "TagsId"));
        }
    }
}