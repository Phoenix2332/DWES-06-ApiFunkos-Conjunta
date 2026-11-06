using System.Threading.Channels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using ApiFunkosConjunta.Common.Configuration;

namespace ApiFunkosConjunta.Services.Email;

/// <summary>
/// Servicio de email que envia los mensajes a traves de SMTP con MailKit.
/// </summary>
public class MailKitEmailService(
    IOptions<SmtpSettings> options,
    ILogger<MailKitEmailService> logger,
    Channel<EmailMessage> emailChannel) : IEmailService {
    private readonly SmtpSettings _settings = options.Value;

    public async Task SendEmailAsync(EmailMessage message) {
        var mimeMessage = ConstruirMimeMessage(message);

        logger.LogInformation("Enviando email a {To} con asunto: {Subject}", message.To, message.Subject);

        try {
            using var smtpClient = new SmtpClient();

            await smtpClient.ConnectAsync(
                _settings.Host,
                _settings.Port,
                GetSecureSocket(_settings.Security));

            if (!string.IsNullOrEmpty(_settings.Username))
                await smtpClient.AuthenticateAsync(_settings.Username, _settings.Password ?? string.Empty);

            await smtpClient.SendAsync(mimeMessage);
            await smtpClient.DisconnectAsync(true);

            logger.LogInformation("Email enviado exitosamente a {To}", message.To);
        } catch (Exception ex) {
            logger.LogError(ex, "Error enviando email a {To}: {Error}", message.To, ex.Message);
            throw;
        }
    }

    public async Task EnqueueEmailAsync(EmailMessage message) {
        await emailChannel.Writer.WriteAsync(message);

        logger.LogInformation("Email encolado para procesamiento en segundo plano a: {To}", message.To);
    }

    private MimeMessage ConstruirMimeMessage(EmailMessage message) {
        var mimeMessage = new MimeMessage();

        mimeMessage.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder();

        if (message.IsHtml)
            bodyBuilder.HtmlBody = message.Body;
        else
            bodyBuilder.TextBody = message.Body;

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        return mimeMessage;
    }

    private static SecureSocketOptions GetSecureSocket(string security) {
        return security?.ToLowerInvariant() switch {
            "ssl" => SecureSocketOptions.SslOnConnect,
            "tls" => SecureSocketOptions.StartTls,
            "none" => SecureSocketOptions.None,
            _ => SecureSocketOptions.Auto
        };
    }
}
