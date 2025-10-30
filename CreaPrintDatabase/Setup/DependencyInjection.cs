using Microsoft.Extensions.DependencyInjection;
using CreaPrintCore.Interfaces;
using CreaPrintDatabase.Repositories;

namespace CreaPrintDatabase.Setup
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDatabaseRepositories(this IServiceCollection services)
        {
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            // Ajoute ici d'autres repositories si besoin
            return services;
        }
    }
}
