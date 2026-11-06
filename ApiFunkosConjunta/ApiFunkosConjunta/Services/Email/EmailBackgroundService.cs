using System.Threading.Channels;

namespace ApiFunkosConjunta.Services.Email;

/// <summary>
/// Servicio en segundo plano que procesa la cola de emails y los envia.
/// Usa Channel para una cola thread-safe.
/// </summary>
public class EmailBackgroundService(
    Channel<EmailMessage> emailChannel,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<EmailBackgroundService> logger) : BackgroundService {
    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        logger.LogInformation("Servicio de email en segundo plano iniciado");

        await foreach (var emailMessage in emailChannel.Reader.ReadAllAsync(stoppingToken)) {
            try {
                // IEmailService es Scoped y el worker es Singleton: creamos un scope por cada envio.
                using var scope = serviceScopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                logger.LogInformation("Procesando email de la cola para: {To}", emailMessage.To);

                await emailService.SendEmailAsync(emailMessage);

                logger.LogInformation("Email procesado exitosamente para: {To}", emailMessage.To);
            } catch (Exception ex) {
                logger.LogError(ex, "Error procesando email para: {To}", emailMessage.To);
            }
        }

        logger.LogInformation("Servicio de email en segundo plano detenido");
    }
}
