using System.Threading.Channels;
using ApiFunkosConjunta.Services.Email;

namespace ApiFunkosConjunta.Infrastructure;

/// <summary>
/// Configuracion del servicio de email.
/// Desarrollo: MemoryEmailService (no envia realmente).
/// Produccion: MailKitEmailService (envia emails reales).
/// </summary>
public static class EmailConfig {
    public static IServiceCollection AddEmail(this IServiceCollection services, IWebHostEnvironment environment) {
        // Cola compartida por toda la aplicacion.
        services.AddSingleton(Channel.CreateUnbounded<EmailMessage>());

        if (environment.IsDevelopment()) {
            services.AddScoped<IEmailService, MemoryEmailService>();
        } else {
            services.AddScoped<IEmailService, MailKitEmailService>();
            services.AddHostedService<EmailBackgroundService>();
        }

        return services;
    }
}
