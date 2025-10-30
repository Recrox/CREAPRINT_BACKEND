using CreaPrintDatabase;
using CreaPrintCore.Setup;
using CreaPrintDatabase.Setup;
using CreaPrintConfiguration.Setup;
using Microsoft.EntityFrameworkCore;
using CreaPrintCore.Models;
using FluentValidation.AspNetCore;
using FluentValidation;
using CreaPrintApi.Validators;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CreaPrintApi.Services;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Ensure logs directory exists
var logsDir = Path.Combine(builder.Environment.ContentRootPath, "_Logs");
Directory.CreateDirectory(logsDir);

// Configure Serilog: create file with format like log-202510_02.txt (yyyyMM_dd)
var fileName = $"log-{DateTime.UtcNow:yyyyMM_dd}.txt";
var logsPath = Path.Combine(logsDir, fileName);
Log.Logger = new LoggerConfiguration()
 .MinimumLevel.Debug()
 .WriteTo.File(logsPath, outputTemplate: "[{Timestamp:yyyyMMdd_HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
 .CreateLogger();

// NOTE: don't call builder.Host.UseSerilog() to avoid extension method mismatch in this environment
// register logger instance in DI so it can be resolved from RequestServices
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

// Configuration des services
builder.Services.AddGlobalConfiguration(builder.Configuration);
// Register database repositories before core services so repository implementations are available when core services are validated
builder.Services.AddDatabaseRepositories();
builder.Services.AddCoreServices();

// Register token blacklist (in-memory)
builder.Services.AddSingleton<ITokenBlacklist, InMemoryTokenBlacklist>();

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

// Configure Swagger (removed Bearer security definition)
builder.Services.AddSwaggerGen(options =>
{
 options.CustomSchemaIds(type => type.FullName);
 // OAuth2 password grant flow for Swagger UI
 options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
 {
 Type = SecuritySchemeType.OAuth2,
 Flows = new OpenApiOAuthFlows
 {
 Password = new OpenApiOAuthFlow
 {
 TokenUrl = new Uri("/api/user/token", UriKind.Relative),
 Scopes = new Dictionary<string, string>
 {
 { "api", "Access API" }
 }
 }
 }
 });
});

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ArticleValidator>();

// JWT configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev_secret_change_me_long_enough";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
 options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
 options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
 options.RequireHttpsMetadata = false;
 options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
 {
 ValidateIssuer = false,
 ValidateAudience = false,
 ValidateIssuerSigningKey = true,
 IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
 ValidateLifetime = true
 };

 // Check token revocation on message received
 options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
 {
 OnMessageReceived = context =>
 {
 // allow the token to be read normally
 return Task.CompletedTask;
 },
 OnTokenValidated = context =>
 {
 var logger = context.HttpContext.RequestServices.GetService<Serilog.ILogger>() ?? Log.Logger;
 var blacklist = context.HttpContext.RequestServices.GetService<ITokenBlacklist>();
 var token = context.SecurityToken as JwtSecurityToken;
 var raw = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();
 logger.Information("Token validated for {Path}, token present: {HasToken}", context.HttpContext.Request.Path, !string.IsNullOrEmpty(raw));
 if (token != null && blacklist != null)
 {
 if (blacklist.IsRevoked(raw ?? string.Empty))
 {
 logger.Warning("Rejected revoked token for {Path}", context.HttpContext.Request.Path);
 context.Fail("Token revoked");
 }
 }
 return Task.CompletedTask;
 }
 };
});

builder.Services.AddAuthorization(options =>
{
 options.AddPolicy("AdminOnly", policy =>
 {
 policy.RequireAssertion(context =>
 {
 var claim = context.User.FindFirst("rights")?.Value;
 if (string.IsNullOrEmpty(claim)) return false;
 if (!int.TryParse(claim, out var rights)) return false;
 // Admin flag ==1
 return (rights & (int)UserRights.Admin) == (int)UserRights.Admin;
 });
 });
});

var app = builder.Build();

// Initialisation de la base InMemory avec des articles de test (dev uniquement)
addFakeDB(app);

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI(options =>
 {
 // Configure OAuth settings so the Swagger UI modal shows username/password for the password flow
 options.OAuthClientId("swagger-ui");
 options.OAuthAppName("CreaPrint Swagger UI");
 options.OAuthUsePkce();
 });
}

app.UseHttpsRedirection();
app.UseCors(); // Ajout du middleware CORS

// Simple explicit request-logging middleware to guarantee entries for each request
app.Use(async (context, next) =>
{
 var sw = Stopwatch.StartNew();
 Log.Logger.Information("Incoming {Method} {Path}", context.Request.Method, context.Request.Path);
 try
 {
 await next();
 }
 finally
 {
 sw.Stop();
 Log.Logger.Information("Finished {Method} {Path} responded {StatusCode} in {Elapsed}ms", context.Request.Method, context.Request.Path, context.Response.StatusCode, sw.Elapsed.TotalMilliseconds);
 }
});

app.UseAuthentication();

// CurrentUser middleware must run after authentication so HttpContext.User is populated
app.UseMiddleware<CreaPrintApi.Middleware.CurrentUserMiddleware>();

app.UseAuthorization();
app.MapControllers();

// Redirige la racine vers Swagger
app.MapGet("/", context => {
 context.Response.Redirect("/swagger");
 return Task.CompletedTask;
});

app.Run();

static void addFakeDB(WebApplication app)
{

 // Initialisation de la base InMemory avec des articles de test (dev uniquement)
 if (app.Environment.IsDevelopment())
 {
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