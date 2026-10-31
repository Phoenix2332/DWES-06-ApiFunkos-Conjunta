using ApiFunkosConjunta.Infrastructure.Persistence;
using ApiFunkosConjunta.Repositories.Implementations;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Implementations;
using ApiFunkosConjunta.Services.Interfaces;

namespace ApiFunkosConjunta.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration) {
        services.AddPersistence(configuration);

        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        services.AddScoped<ICategoriaService, CategoriaService>();

        return services;
    }
}