using CreaPrintCore.Models;
using Microsoft.EntityFrameworkCore;

namespace CreaPrintDatabase
{
 public class CreaPrintDbContext : DbContext
 {
 public CreaPrintDbContext(DbContextOptions<CreaPrintDbContext> options) : base(options) { }

 public DbSet<Article> Articles { get; set; }
 }
}
