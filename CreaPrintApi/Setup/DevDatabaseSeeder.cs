using CreaPrintDatabase;
using CreaPrintCore.Models;

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
 new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m, Stock =20 },
 new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m, Stock =15 },
 new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m, Stock =30 },
 new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m, Stock =12 },
 new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m, Stock =25 },
 new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m, Stock =8 },
 new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m, Stock =10 },
 new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m, Stock =5 },
 new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m, Stock =18 },
 new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m, Stock =7 },
 new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m, Stock =22 },
 new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m, Stock =3 },
 new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m, Stock =14 },
 new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m, Stock =9 },
 new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m, Stock =6 },
 new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m, Stock =11 },
 new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m, Stock =13 },
 new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m, Stock =4 },
 new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m, Stock =16 },
 new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m, Stock =2 },
 new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m, Stock =19 },
 new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m, Stock =21 },
 new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m, Stock =17 },
 new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m, Stock =1 },
 new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m, Stock =14 },
 new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m, Stock =6 },
 new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m, Stock =27 },
 new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m, Stock =5 },
 new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m, Stock =8 },
 new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m, Stock =9 },
 });
 db.SaveChanges();
 }

 // create fake admin user if not exists
 if (!db.Users.Any())
 {
 var admin = new User { Username = "admin", Rights = UserRights.Admin | UserRights.Vendeur | UserRights.Artisan | UserRights.Commercial, Email = "admin@local" };
 userService.CreateAsync(admin, "admin123").GetAwaiter().GetResult();
 }
 }
}
