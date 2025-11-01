using CreaPrintDatabase;
using CreaPrintCore.Models;
using System.Collections.Generic;
using System;

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

 var articles = new[]
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
 };

 db.Articles.AddRange(articles);
 db.SaveChanges();

 // create simple images for each article
 var images = new List<ArticleImage>();
 var imageUrls = new[]
 {
 "https://picsum.photos/seed/pic1/600/400",
 "https://picsum.photos/seed/pic2/600/400",
 "https://picsum.photos/seed/pic3/600/400",
 "https://picsum.photos/seed/pic4/600/400",
 "https://picsum.photos/seed/pic5/600/400",
 "https://picsum.photos/seed/pic6/600/400",
 "https://picsum.photos/seed/pic7/600/400",
 "https://picsum.photos/seed/pic8/600/400",
 "https://picsum.photos/seed/pic9/600/400",
 "https://picsum.photos/seed/pic10/600/400",
 "https://picsum.photos/seed/pic11/600/400",
 "https://picsum.photos/seed/pic12/600/400",
 "https://picsum.photos/seed/pic13/600/400",
 "https://picsum.photos/seed/pic14/600/400",
 "https://picsum.photos/seed/pic15/600/400",
 "https://picsum.photos/seed/pic16/600/400",
 "https://picsum.photos/seed/pic17/600/400",
 "https://picsum.photos/seed/pic18/600/400",
 "https://picsum.photos/seed/pic19/600/400",
 "https://picsum.photos/seed/pic20/600/400",
 };

 for (int i =0; i < articles.Length; i++)
 {
 var a = articles[i];
 images.Add(new ArticleImage { ArticleId = a.Id, Url = $"/images/article_{i+1}.jpg", IsPrimary = true });
 images.Add(new ArticleImage { ArticleId = a.Id, Url = imageUrls[i % imageUrls.Length], IsPrimary = true });
 }
 db.ArticleImages.AddRange(images);
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
