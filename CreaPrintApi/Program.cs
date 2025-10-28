using CreaPrintDatabase;
using CreaPrintDatabase.Repositories;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Services;
using CreaPrintConfiguration.Setup;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Global configuration setup
builder.Services.AddGlobalConfiguration(builder.Configuration);


builder.Services.AddDbContext<CreaPrintDbContext>(options =>
 options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
