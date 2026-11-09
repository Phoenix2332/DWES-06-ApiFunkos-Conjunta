using System.Reflection;
using ApiFunkosConjunta.Common.Examples;
using ApiFunkosConjunta.Infrastructure.Filters;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Infrastructure;

/// <summary>
/// Extensiones de configuracion de Swagger/OpenAPI (punto 24 del tema).
/// </summary>
public static class SwaggerConfig {
    /// <summary>
    /// Registra el generador de OpenAPI de Swashbuckle con la informacion de la API, el esquema de
    /// seguridad JWT, los comentarios XML, los atributos de documentacion, los ejemplos de peticion
    /// y el filtro personalizado de operaciones.
    /// </summary>
    public static IServiceCollection AddSwagger(this IServiceCollection services) {
        // Necesario para que Swashbuckle vea los endpoints de los controllers.
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options => {
            // Informacion general de la API (apartado 24.2.4).
            options.SwaggerDoc("v1", new OpenApiInfo {
                Title = "ApiFunkosConjunta API",
                Version = "v1.0.0",
                Description =
                    "API REST para la gestion del catalogo de Funkos. " +
                    "Persistencia en PostgreSQL (categorias y funkos), MongoDB (proveedores) y Redis (cache). " +
                    "Los endpoints de escritura requieren el rol Administrador.",
                Contact = new OpenApiContact {
                    Name = "ApiFunkosConjunta",
                    Email = "admin@apifunkos.com"
                }
            });

            // Esquema de seguridad JWT: habilita el boton "Authorize" de Swagger UI (apartado 24.5).
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Introduce el token JWT con el formato 'Bearer' + espacio + token."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement {
                {
                    new OpenApiSecuritySchemeReference("Bearer", document),
                    new List<string>()
                }
            });

            // Comentarios XML de los controllers, DTOs y servicios (apartado 24.2.4).
            // Solo se incluyen si el fichero existe: si no, IncludeXmlComments lanza FileNotFoundException.
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            // Atributos de documentacion: SwaggerOperation, SwaggerResponse, SwaggerSchema...
            options.EnableAnnotations();

            // Ejemplos de peticion con IExamplesProvider (apartado 24.8).
            options.ExampleFilters();

            // Filtro personalizado de operaciones (apartado 24.9).
            options.OperationFilter<SwaggerExamplesFilter>();
        });

        // Registro de los IExamplesProvider<T> del ensamblado (apartado 24.8).
        services.AddSwaggerExamplesFromAssemblyOf<CrearFunkoDtoExample>();

        return services;
    }
}
