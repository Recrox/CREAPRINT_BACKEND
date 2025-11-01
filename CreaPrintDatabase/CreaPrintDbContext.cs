using CreaPrintCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase
{
    public class CreaPrintDbContext : DbContext
    {
        public CreaPrintDbContext(DbContextOptions<CreaPrintDbContext> options) : base(options) { }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketItem> BasketItems { get; set; }
        public DbSet<ArticleImage> ArticleImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>()
            .HasOne(a => a.Category)
            .WithMany(c => c.Articles)
            .HasForeignKey(a => a.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Basket>()
            .HasMany(b => b.Items)
            .WithOne(i => i.Basket)
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ArticleImage>()
            .HasOne(ai => ai.Article)
            .WithMany(a => a.Images)
            .HasForeignKey(ai => ai.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
