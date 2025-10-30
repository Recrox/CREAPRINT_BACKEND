using CreaPrintDatabase;
using CreaPrintCore.Setup;
using CreaPrintDatabase.Setup;
using CreaPrintConfiguration.Setup;
using Microsoft.EntityFrameworkCore;
using CreaPrintCore.Models;
using FluentValidation.AspNetCore;
using FluentValidation;
using CreaPrintApi.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services
builder.Services.AddGlobalConfiguration(builder.Configuration);
// Register database repositories before core services so repository implementations are available when core services are validated
builder.Services.AddDatabaseRepositories();
builder.Services.AddCoreServices();

// Lecture des AllowedHosts pour CORS
IEnumerable<string> allowedHosts = builder.Configuration["AllowedHosts"]?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedHosts.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Choix du provider de base de données (InMemory pour dev, SQL Server pour prod)
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<CreaPrintDbContext>(options =>
        options.UseInMemoryDatabase("MockDb"));
}
else
{
    builder.Services.AddDbContext<CreaPrintDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
}

builder.Services.AddControllers()
 .AddJsonOptions(options =>
 {
 options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
 })
 .AddNewtonsoftJson(options =>
 {
 // Ensure Newtonsoft ignores reference loops for JsonPatch and swagger generation
 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
 });
builder.Services.AddAutoMapper(typeof(CreaPrintApi.Dtos.MappingProfile));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ArticleValidator>();

var app = builder.Build();

// Initialisation de la base InMemory avec des articles de test (dev uniquement)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CreaPrintDbContext>();
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
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
new Article { Title = "Premier article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =10.99m },
            new Article { Title = "Second article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =15.50m },
            new Article { Title = "3 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =8.75m },
            new Article { Title = "4 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =12.00m },
            new Article { Title = "5 iem article", Content = "Contenu de test", CategoryId = testCategory.Id, Category = testCategory, CreatedOn = DateTime.Now, Price =9.99m },
            new Article { Title = "6 iem article", Content = "Encore du contenu", CategoryId = demoCategory.Id, Category = demoCategory, CreatedOn = DateTime.Now, Price =20.00m },
        });
        db.SaveChanges();
    }
}

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(); // Ajout du middleware CORS
app.UseAuthorization();
app.MapControllers();

// Redirige la racine vers Swagger
app.MapGet("/", context => {
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();
