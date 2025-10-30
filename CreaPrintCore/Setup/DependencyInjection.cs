using Microsoft.Extensions.DependencyInjection;
using CreaPrintCore.Interfaces;
using CreaPrintCore.Services;

namespace CreaPrintCore.Setup
{
 public static class DependencyInjection
 {
 public static IServiceCollection AddCoreServices(this IServiceCollection services)
 {
 services.AddScoped<IArticleService, ArticleService>();
 services.AddScoped<IUserService, UserService>();
 services.AddScoped<CurrentUser>();
 // Ajoute ici d'autres services du core si besoin
 return services;
 }
 }
}
