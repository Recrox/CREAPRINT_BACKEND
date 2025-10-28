using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CreaPrintConfiguration.Settings;

namespace CreaPrintConfiguration.Setup
{
 public static class ServiceSetup
 {
 public static IServiceCollection AddGlobalConfiguration(this IServiceCollection services, IConfiguration configuration)
 {
 services.Configure<DatabaseSettings>(configuration.GetSection("ConnectionStrings"));
 return services;
 }
 }
}
