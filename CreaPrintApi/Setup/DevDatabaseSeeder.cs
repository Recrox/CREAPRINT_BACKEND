using Microsoft.EntityFrameworkCore;
using CreaPrintDatabase;
using CreaPrintCore.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CreaPrintApi.Setup;

public static class DevDatabaseSeeder
{
 public static void SeedFakeData(WebApplication app)
 {
 if (!app.Environment.IsDevelopment()) return;

 using var scope = app.Services.CreateScope();
 var db = scope.ServiceProvider.GetRequiredService<CreaPrintDbContext>();
 var userService = scope.ServiceProvider.GetRequiredService<CreaPrintCore.Interfaces.IUserService>();
 if (!db.Articles.Any())
 {
 var testCategory = new Category { Name = "Test" };
 var demoCategory = new Category { Name = "Demo" };
 db.Categories.AddRange(testCategory, demoCategory);
 db.SaveChanges();

 db.Articles.AddRange(new[]
 {
 new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
 new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
 new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
 new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
 new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
 new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
 });
 db.SaveChanges();
 }

 // create fake admin user if not exists
 if (!db.Users.Any())
 {
 var admin = new CreaPrintCore.Models.User { Username = "admin", Rights = CreaPrintCore.Models.UserRights.Admin };
 userService.CreateAsync(admin, "admin123").GetAwaiter().GetResult();
 }
 }
}
