using ApiFunkosConjunta.Services.Email;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Email;

[TestFixture]
public class MemoryEmailServiceTest {
    private Mock<ILogger<MemoryEmailService>> _logger = null!;
    private MemoryEmailService _service = null!;

    [SetUp]
    public void SetUp() {
        _logger = new Mock<ILogger<MemoryEmailService>>();
        _service = new MemoryEmailService(_logger.Object);
        MemoryEmailService.Clear();
    }

    [Test]
    public async Task SendEmailAsync_EmailValido_RegistraElEmailEnviado() {
        // Arrange
        var email = new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Nuevo funko: Funko de prueba",
            Body = "<p>Contenido</p>",
            IsHtml = true
        };

        // Act
        await _service.SendEmailAsync(email);

        // Assert
        MemoryEmailService.SentEmails.Should().HaveCount(1);
        MemoryEmailService.SentEmails[0].To.Should().Be("admin@apifunkos.com");
        MemoryEmailService.SentEmails[0].Subject.Should().Be("Nuevo funko: Funko de prueba");
        MemoryEmailService.SentEmails[0].Body.Should().Be("<p>Contenido</p>");
        MemoryEmailService.SentEmails[0].IsHtml.Should().BeTrue();
    }

    [Test]
    public async Task EnqueueEmailAsync_EmailValido_RegistraElEmailEncolado() {
        // Arrange
        var email = new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Nuevo funko: Funko de prueba",
            Body = "<p>Contenido</p>",
            IsHtml = true
        };

        // Act
        await _service.EnqueueEmailAsync(email);

        // Assert
        MemoryEmailService.EnqueuedEmails.Should().HaveCount(1);
        MemoryEmailService.EnqueuedEmails[0].To.Should().Be("admin@apifunkos.com");
        MemoryEmailService.EnqueuedEmails[0].Subject.Should().Be("Nuevo funko: Funko de prueba");
    }

    [Test]
    public async Task SendEmailAsync_ConVariosEmails_RegistraTodosLosEnviados() {
        // Arrange
        var email1 = new EmailMessage { To = "uno@apifunkos.com", Subject = "Uno", Body = "1", IsHtml = false };
        var email2 = new EmailMessage { To = "dos@apifunkos.com", Subject = "Dos", Body = "2", IsHtml = false };

        // Act
        await _service.SendEmailAsync(email1);
        await _service.SendEmailAsync(email2);

        // Assert
        MemoryEmailService.SentEmails.Should().HaveCount(2);
        MemoryEmailService.SentEmails.Should().Contain(e => e.To == "uno@apifunkos.com");
        MemoryEmailService.SentEmails.Should().Contain(e => e.To == "dos@apifunkos.com");
    }

    [Test]
    public async Task GetLastEmail_DestinatarioConocido_RetornaElUltimoEmailEnviado() {
        // Arrange
        await _service.SendEmailAsync(new EmailMessage {
            To = "admin@apifunkos.com", Subject = "Primero", Body = "1", IsHtml = false
        });
        await _service.SendEmailAsync(new EmailMessage {
            To = "admin@apifunkos.com", Subject = "Ultimo", Body = "2", IsHtml = false
        });

        // Act
        var email = MemoryEmailService.GetLastEmail("admin@apifunkos.com");

        // Assert
        email.Should().NotBeNull();
        email!.Subject.Should().Be("Ultimo");
    }

    [Test]
    public async Task GetLastEnqueuedEmail_DestinatarioConocido_RetornaElUltimoEmailEncolado() {
        // Arrange
        await _service.EnqueueEmailAsync(new EmailMessage {
            To = "admin@apifunkos.com", Subject = "Encolado", Body = "1", IsHtml = false
        });

        // Act
        var email = MemoryEmailService.GetLastEnqueuedEmail("admin@apifunkos.com");

        // Assert
        email.Should().NotBeNull();
        email!.Subject.Should().Be("Encolado");
    }

    [Test]
    public async Task Clear_ConEmailsRegistrados_LosVacia() {
        // Arrange
        await _service.SendEmailAsync(new EmailMessage {
            To = "admin@apifunkos.com", Subject = "Enviado", Body = "1", IsHtml = false
        });
        await _service.EnqueueEmailAsync(new EmailMessage {
            To = "admin@apifunkos.com", Subject = "Encolado", Body = "2", IsHtml = false
        });

        // Act
        MemoryEmailService.Clear();

        // Assert
        MemoryEmailService.SentEmails.Should().BeEmpty();
        MemoryEmailService.EnqueuedEmails.Should().BeEmpty();
    }

    [Test]
    public async Task SendEmailAsync_EmailValido_RegistraElEnvioEnElLog() {
        // Arrange
        var email = new EmailMessage {
            To = "admin@apifunkos.com", Subject = "Nuevo funko: Funko de prueba", Body = "<p>Contenido</p>",
            IsHtml = true
        };

        // Act
        await _service.SendEmailAsync(email);

        // Assert
        _logger.Verify(l => l.Log(
            LogLevel.Information,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v != null && v.ToString()!.Contains("SENT")),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), Times.Once);
    }
}
