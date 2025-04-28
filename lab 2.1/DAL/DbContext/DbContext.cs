//using Microsoft.EntityFrameworkCore;
//using BulletinBoard.DAL.Models;

//namespace BulletinBoard.DAL
//{
//    public class BulletinBoardContext : DbContext
//    {
//        public DbSet<User> Users { get; set; }
//        public DbSet<Category> Categories { get; set; }
//        public DbSet<Tag> Tags { get; set; }
//        public DbSet<Ad> Ads { get; set; }

//        // Стандартний конструктор з опціями
//        public BulletinBoardContext(DbContextOptions<BulletinBoardContext> options)
//            : base(options)
//        {
//        }

//        //конструктор без параметрів для використання рядка підключення за замовчуванням
//        public BulletinBoardContext()
//        {
//        }

//        // Метод для налаштування опцій та рядка підключення
//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//                string connectionString = "Server=localhost\\SQLEXPRESS;Database=BulletinBoardDB;Trusted_Connection=True;TrustServerCertificate=True;";
//                optionsBuilder.UseSqlServer(connectionString);
//            }
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            base.OnModelCreating(modelBuilder);

//            // Налаштування зовнішніх ключів та відносин для Ad
//            modelBuilder.Entity<Ad>()
//                .HasOne(a => a.User)
//                .WithMany(u => u.Ads)
//                .HasForeignKey(a => a.UserId)
//                .OnDelete(DeleteBehavior.Cascade);

//            modelBuilder.Entity<Ad>()
//                .HasOne(a => a.Category)
//                .WithMany(c => c.Ads)
//                .HasForeignKey(a => a.CategoryId)
//                .OnDelete(DeleteBehavior.Restrict);

//            // Налаштування відносин для Category з самою собою (ієрархія)
//            modelBuilder.Entity<Category>()
//                .HasOne(c => c.ParentCategory)
//                .WithMany(c => c.SubCategories)
//                .HasForeignKey(c => c.ParentCategoryId)
//                .OnDelete(DeleteBehavior.Restrict)
//                .IsRequired(false);

//            // Налаштування для багато-до-багатьох між Ads і Tags
//            modelBuilder.Entity<Ad>()
//                .HasMany(a => a.Tags)
//                .WithMany(t => t.Ads)
//                .UsingEntity(j => j
//                    .ToTable("AdTags")
//                    .HasKey("AdsId", "TagsId"));
//        }
//    }
//}