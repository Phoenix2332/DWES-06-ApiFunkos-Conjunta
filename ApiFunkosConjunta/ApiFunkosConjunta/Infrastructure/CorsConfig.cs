using Microsoft.AspNetCore.Builder;

namespace ApiFunkosConjunta.Infrastructure;

/// <summary>
/// Extensiones de configuracion de CORS (apartado 24.1 del tema).
/// En desarrollo se permite cualquier origen; en produccion solo los origenes configurados.
/// </summary>
public static class CorsConfig {
    /// <summary>
    /// Nombre de la politica permisiva, usada solo en desarrollo.
    /// </summary>
    public const string AllowAllPolicy = "AllowAll";

    /// <summary>
    /// Nombre de la politica restrictiva, usada en produccion.
    /// </summary>
    public const string ProductionPolicy = "ProductionPolicy";

    /// <summary>
    /// Registra la politica de CORS segun el entorno. En produccion los origenes permitidos se
    /// leen de la seccion de configuracion <c>Cors:AllowedOrigins</c>.
    /// </summary>
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration,
        bool isDevelopment) {
        return services.AddCors(options => {
            if (isDevelopment) {
                options.AddPolicy(AllowAllPolicy, policy => {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });

                return;
            }

            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                                 ?? throw new InvalidOperationException(
                                     "No se ha configurado la seccion 'Cors:AllowedOrigins'.");

            options.AddPolicy(ProductionPolicy, policy => {
                policy.WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }

    /// <summary>
    /// Aplica al pipeline la politica de CORS que corresponde al entorno.
    /// </summary>
    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app, bool isDevelopment) {
        return app.UseCors(isDevelopment ? AllowAllPolicy : ProductionPolicy);
    }
}
