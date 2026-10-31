using ApiFunkosConjunta.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiFunkosConjunta.Infrastructure.Persistence;

public static class PersistenceExtensions {
    public static IServiceCollection AddPersistence(this IServiceCollection services,
        IConfiguration configuration) {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSQL"))
        );

        services.AddStackExchangeRedisCache(options => {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}