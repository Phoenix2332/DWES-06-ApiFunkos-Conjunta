using System.Threading.Channels;
using ApiFunkosConjunta.Common.Configuration;
using ApiFunkosConjunta.Services.Email;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Email;

[TestFixture]
public class MailKitEmailServiceTest {
    [SetUp]
    public void SetUp() {
        _logger = new Mock<ILogger<MailKitEmailService>>();

        _emailChannel = Channel.CreateUnbounded<EmailMessage>();

        var settings = Options.Create(new SmtpSettings {
            Host = "localhost",
            Port = 1025,
            FromEmail = "noreply@apifunkos.com",
            FromName = "ApiFunkos",
            Security = "None"
        });

        _service = new MailKitEmailService(
            settings,
            _logger.Object,
            _emailChannel
        );
    }

    private Mock<ILogger<MailKitEmailService>> _logger = null!;
    private Channel<EmailMessage> _emailChannel = null!;
    private MailKitEmailService _service = null!;

    [Test]
    public async Task SendEmailAsync_EmailHtmlValido_EnviaCorreo() {
        // Arrange
        var email = new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Prueba HTML",
            Body = "<h1>Correo de prueba</h1>",
            IsHtml = true
        };

        // Act
        var accion = async () => await _service.SendEmailAsync(email);

        // Assert
        await accion.Should().NotThrowAsync();
    }

    [Test]
    public async Task SendEmailAsync_EmailTextoValido_EnviaCorreo() {
        // Arrange
        var email = new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Prueba texto",
            Body = "Correo de prueba en texto plano",
            IsHtml = false
        };

        // Act
        var accion = async () => await _service.SendEmailAsync(email);

        // Assert
        await accion.Should().NotThrowAsync();
    }

    [Test]
    public async Task SendEmailAsync_ServidorNoDisponible_LanzaExcepcion() {
        // Arrange
        var settings = Options.Create(new SmtpSettings {
            Host = "localhost",
            Port = 1,
            FromEmail = "noreply@apifunkos.com",
            FromName = "ApiFunkos",
            Security = "None"
        });

        var service = new MailKitEmailService(
            settings,
            _logger.Object,
            _emailChannel
        );

        var email = new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Prueba error",
            Body = "Contenido",
            IsHtml = false
        };

        // Act
        var accion = async () => await service.SendEmailAsync(email);

        // Assert
        await accion.Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task EnqueueEmailAsync_EmailValido_EncolaMensaje() {
        // Arrange
        var email = new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Correo encolado",
            Body = "Contenido de prueba",
            IsHtml = true
        };

        // Act
        await _service.EnqueueEmailAsync(email);

        // Assert
        _emailChannel.Reader.TryRead(out var mensaje).Should().BeTrue();

        mensaje.Should().NotBeNull();
        mensaje!.To.Should().Be(email.To);
        mensaje.Subject.Should().Be(email.Subject);
        mensaje.Body.Should().Be(email.Body);
        mensaje.IsHtml.Should().BeTrue();
    }
}