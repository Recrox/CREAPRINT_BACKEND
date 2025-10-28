using CreaPrintDatabase;
using CreaPrintCore.Setup;
using CreaPrintDatabase.Setup;
using CreaPrintConfiguration.Setup;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration des services
builder.Services.AddGlobalConfiguration(builder.Configuration);
builder.Services.AddCoreServices();
builder.Services.AddDatabaseRepositories();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialisation de la base InMemory avec des articles de test (dev uniquement)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CreaPrintDbContext>();
    if (!db.Articles.Any())
    {
        db.Articles.AddRange(new[]
        {
            new CreaPrintCore.Models.Article { Title = "Premier article", Content = "Contenu de test", Category = "Test", CreatedAt = DateTime.Now },
            new CreaPrintCore.Models.Article { Title = "Second article", Content = "Encore du contenu", Category = "Demo", CreatedAt = DateTime.Now }
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
app.UseAuthorization();
app.MapControllers();

// Redirige la racine vers Swagger
app.MapGet("/", context => {
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();
