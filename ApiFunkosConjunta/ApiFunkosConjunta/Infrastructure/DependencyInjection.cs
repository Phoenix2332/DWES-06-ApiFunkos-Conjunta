using System.Text;
using ApiFunkosConjunta.Infrastructure.Persistence;
using ApiFunkosConjunta.Repositories.Implementations;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services;
using ApiFunkosConjunta.Services.Implementations;
using ApiFunkosConjunta.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ApiFunkosConjunta.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration) {
        services.AddPersistence(configuration);

        var jwtKey = configuration["Jwt:Key"]
                     ?? throw new InvalidOperationException("No se ha configurado la clave JWT.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey))
                };
            });

        services.AddAuthorization();

        services.AddScoped<ICategoriaRepository, CategoriaRepository>();

        services.AddScoped<ICategoriaService, CategoriaService>();

        services.AddScoped<IProveedorRepository, ProveedorRepository>();

        services.AddScoped<IProveedorService, ProveedorService>();

        services.AddScoped<IFunkoRepository, FunkoRepository>();

        services.AddScoped<IFunkoService, FunkoService>();

        services.AddScoped<ICatalogoService, CatalogoService>();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}