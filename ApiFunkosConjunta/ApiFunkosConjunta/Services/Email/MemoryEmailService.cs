using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace ApiFunkosConjunta.Services.Email;

/// <summary>
/// Implementacion de IEmailService para desarrollo y testing que solo loguea los emails.
/// No necesita servidor SMTP: los mensajes se guardan en memoria para poder verificarlos.
/// </summary>
public class MemoryEmailService(ILogger<MemoryEmailService> logger) : IEmailService {
    // ConcurrentQueue: el BackgroundService puede escribir mientras los tests leen, y ademas
    // mantiene el orden de insercion (FIFO), necesario para saber cual es el ultimo email.
    private static readonly ConcurrentQueue<EmailMessage> _sentEmails = new();
    private static readonly ConcurrentQueue<EmailMessage> _enqueuedEmails = new();

    /// <summary>Copia de los emails enviados (thread-safe para los asserts).</summary>
    public static IReadOnlyList<EmailMessage> SentEmails => _sentEmails.ToList();

    /// <summary>Copia de los emails encolados (thread-safe para los asserts).</summary>
    public static IReadOnlyList<EmailMessage> EnqueuedEmails => _enqueuedEmails.ToList();

    public Task SendEmailAsync(EmailMessage message) {
        LogEmail(message, "SENT");
        _sentEmails.Enqueue(message);
        return Task.CompletedTask;
    }

    public Task EnqueueEmailAsync(EmailMessage message) {
        LogEmail(message, "ENQUEUED");
        _enqueuedEmails.Enqueue(message);
        return Task.CompletedTask;
    }

    /// <summary>Vacia los emails registrados.</summary>
    public static void Clear() {
        while (_sentEmails.TryDequeue(out _)) {
        }

        while (_enqueuedEmails.TryDequeue(out _)) {
        }
    }

    /// <summary>Ultimo email enviado al destinatario indicado.</summary>
    public static EmailMessage? GetLastEmail(string to) {
        return SentEmails.LastOrDefault(e => e.To.Equals(to, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Ultimo email encolado para el destinatario indicado.</summary>
    public static EmailMessage? GetLastEnqueuedEmail(string to) {
        return EnqueuedEmails.LastOrDefault(e => e.To.Equals(to, StringComparison.OrdinalIgnoreCase));
    }

    private void LogEmail(EmailMessage message, string status) {
        logger.LogInformation("=== EMAIL {Status} ===", status);
        logger.LogInformation("Para: {To}", message.To);
        logger.LogInformation("Asunto: {Subject}", message.Subject);
        logger.LogInformation("Tipo: {Type}", message.IsHtml ? "HTML" : "Texto plano");
        logger.LogDebug("Cuerpo: {Body}", message.Body);
        logger.LogInformation("======================");
    }
}
