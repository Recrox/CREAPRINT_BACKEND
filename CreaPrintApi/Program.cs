using CreaPrintDatabase;
using CreaPrintCore.Setup;
using CreaPrintDatabase.Setup;
using CreaPrintConfiguration.Setup;
using Microsoft.EntityFrameworkCore;
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
using System.Text.Json.Serialization;
using CreaPrintCore.Interfaces;
using Swashbuckle.AspNetCore.SwaggerUI;
using CreaPrintConfiguration.Settings;
using Microsoft.Extensions.Options;
using CreaPrintCore.Models.Users;

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

// integrate Serilog with generic host logging so Microsoft ILogger logs go to Serilog
// NOTE: avoid UseSerilog extension mismatch in this environment
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

// register logger instance in DI so it can be resolved from RequestServices
builder.Services.AddSingleton<Serilog.ILogger>(Log.Logger);

// Configuration des services
builder.Services.AddGlobalConfiguration(builder.Configuration);
// Register database repositories before core services so repository implementations are available when core services are validated
builder.Services.AddDatabaseRepositories();
builder.Services.AddCoreServices();

// Register token blacklist (in-memory)
builder.Services.AddSingleton<ITokenBlacklist, InMemoryTokenBlacklist>();

// Register email service
builder.Services.AddSingleton<IEmailService, EmailService>();

// Lecture des AllowedHosts pour CORS
IEnumerable<string> allowedHosts = builder.Configuration["AllowedHosts"]?.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
 options.AddDefaultPolicy(policy =>
 {
 policy.WithOrigins(allowedHosts.ToArray())
 .AllowAnyHeader()
 .AllowAnyMethod();
 });
});

// Bind GlobalSettings for direct DI access and register typed options
builder.Services.Configure<GlobalSettings>(builder.Configuration.GetSection("GlobalSettings"));
var gs = builder.Configuration.GetSection("GlobalSettings").Get<GlobalSettings>() ?? new GlobalSettings();
builder.Services.AddSingleton(gs);

// Register named HttpClient based on GlobalSettings.ApiUrl
if (!string.IsNullOrWhiteSpace(gs.ApiUrl))
{
 builder.Services.AddHttpClient("CreaPrintApiClient", client => client.BaseAddress = new Uri(gs.ApiUrl));
}

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
 // Serialize enums as strings for System.Text.Json
 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
 })
 .AddNewtonsoftJson(options =>
 {
 // Ensure Newtonsoft ignores reference loops for JsonPatch and swagger generation
 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
 // Serialize enums as strings for Newtonsoft
 options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
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

 // Also add a simple Bearer scheme so the user can paste a token via the Authorize button
 options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
 {
 Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
 Name = "Authorization",
 In = ParameterLocation.Header,
 Type = SecuritySchemeType.Http,
 Scheme = "bearer",
 BearerFormat = "JWT"
 });

 // Apply both schemes globally (so endpoints show lock icon)
 options.AddSecurityRequirement(new OpenApiSecurityRequirement
 {
 {
 new OpenApiSecurityScheme
 {
 Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
 }, new string[] { }
 },
 {
 new OpenApiSecurityScheme
 {
 Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
 }, new[] { "api" }
 }
 });

 // Ensure enums are represented as strings in Swagger schema
 options.SchemaFilter<CreaPrintApi.Swagger.EnumSchemaFilter>();
});

// Configure FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<ArticleValidator>();

// JWT configuration
var jwtKey = builder.Configuration.GetValue<string>("Jwt:Key");
if (string.IsNullOrEmpty(jwtKey))
{
 Log.Logger.Warning("Configuration key 'Jwt:Key' is missing. Falling back to default development key. Set Jwt:Key in appsettings for production.");
 jwtKey = "dev_secret_change_me_long_enough";
}
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
 //logger.Information("Token validated for {Path}, token present: {HasToken}", context.HttpContext.Request.Path, !string.IsNullOrEmpty(raw));
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
CreaPrintApi.Setup.DevDatabaseSeeder.SeedFakeData(app);

// Pipeline HTTP
// Enable Swagger and SwaggerUI in all environments (previously only in Development)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
 // Configure OAuth settings so the Swagger UI modal shows username/password for the password flow
 options.OAuthClientId("swagger-ui");
 options.OAuthAppName("CreaPrint Swagger UI");
 options.OAuthUsePkce();
 // Collapse operations and tags by default
 options.DocExpansion(DocExpansion.None);
});

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
app.MapGet("/", context =>
{
 context.Response.Redirect("/swagger");
 return Task.CompletedTask;
});

app.Run();