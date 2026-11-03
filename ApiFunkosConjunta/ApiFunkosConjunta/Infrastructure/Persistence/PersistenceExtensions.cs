using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

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

        // MongoDB: el MongoClient es singleton porque es thread-safe y gestiona su propio pool de conexiones.
        services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(configuration["MongoDB:ConnectionString"]));

        services.AddSingleton<IMongoDatabase>(provider =>
            provider.GetRequiredService<IMongoClient>()
                .GetDatabase(configuration["MongoDB:DatabaseName"]));

        services.AddSingleton<IMongoCollection<Proveedor>>(provider =>
            provider.GetRequiredService<IMongoDatabase>()
                .GetCollection<Proveedor>("proveedores"));

        return services;
    }
}