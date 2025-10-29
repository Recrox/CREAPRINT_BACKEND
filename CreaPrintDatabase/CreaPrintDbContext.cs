using CreaPrintCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase
{
 public class CreaPrintDbContext : DbContext
 {
 public CreaPrintDbContext(DbContextOptions<CreaPrintDbContext> options) : base(options) { }

 public DbSet<Article> Articles { get; set; }
 public DbSet<Category> Categories { get; set; }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
 base.OnModelCreating(modelBuilder);

 modelBuilder.Entity<Article>()
 .HasOne(a => a.Category)
 .WithMany(c => c.Articles)
 .HasForeignKey(a => a.CategoryId)
 .OnDelete(DeleteBehavior.Restrict);
 }
 }
}
