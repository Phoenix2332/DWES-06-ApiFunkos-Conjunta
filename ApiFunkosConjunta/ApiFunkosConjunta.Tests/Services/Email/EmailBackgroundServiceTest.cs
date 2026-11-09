using System.Threading.Channels;
using ApiFunkosConjunta.Services.Email;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Email;

[TestFixture]
public class EmailBackgroundServiceTest {
    private Channel<EmailMessage> _channel = null!;
    private EmailBackgroundService _worker = null!;
    private CancellationTokenSource _cts = null!;

    [SetUp]
    public void SetUp() {
        MemoryEmailService.Clear();

        _channel = Channel.CreateUnbounded<EmailMessage>();
        _cts = new CancellationTokenSource();

        // El worker resuelve IEmailService desde un scope: se simula con un IServiceScopeFactory.
        var emailService = new MemoryEmailService(new Mock<ILogger<MemoryEmailService>>().Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(p => p.GetService(typeof(IEmailService))).Returns(emailService);

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        _worker = new EmailBackgroundService(
            _channel,
            scopeFactoryMock.Object,
            new Mock<ILogger<EmailBackgroundService>>().Object);
    }

    [TearDown]
    public async Task TearDown() {
        await _worker.StopAsync(CancellationToken.None);
        _worker.Dispose();
        _cts.Dispose();
    }

    [Test]
    public async Task ExecuteAsync_EmailEnLaCola_LoProcesaYLoEnvia() {
        // Arrange
        var email = new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Nuevo funko: Funko de prueba",
            Body = "<p>Contenido</p>",
            IsHtml = true
        };

        await _worker.StartAsync(_cts.Token);

        // Act
        await _channel.Writer.WriteAsync(email);
        await EsperarA(() => MemoryEmailService.SentEmails.Any(e => e.To == email.To));

        // Assert
        MemoryEmailService.SentEmails.Should().HaveCount(1);
        MemoryEmailService.SentEmails[0].Subject.Should().Be("Nuevo funko: Funko de prueba");
        MemoryEmailService.SentEmails[0].IsHtml.Should().BeTrue();
    }

    [Test]
    public async Task ExecuteAsync_ElEnvioFalla_RegistraElErrorYNoDetieneElWorker() {
        // Arrange
        var emailService = new Mock<IEmailService>();

        emailService
            .Setup(s => s.SendEmailAsync(It.IsAny<EmailMessage>()))
            .ThrowsAsync(new InvalidOperationException("El servidor SMTP no esta disponible."));

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock.Setup(p => p.GetService(typeof(IEmailService))).Returns(emailService.Object);

        var scopeMock = new Mock<IServiceScope>();
        scopeMock.Setup(s => s.ServiceProvider).Returns(serviceProviderMock.Object);

        var scopeFactoryMock = new Mock<IServiceScopeFactory>();
        scopeFactoryMock.Setup(f => f.CreateScope()).Returns(scopeMock.Object);

        var logger = new Mock<ILogger<EmailBackgroundService>>();

        var worker = new EmailBackgroundService(_channel, scopeFactoryMock.Object, logger.Object);

        await worker.StartAsync(_cts.Token);

        // Act
        await _channel.Writer.WriteAsync(new EmailMessage {
            To = "admin@apifunkos.com",
            Subject = "Con error",
            Body = "Contenido",
            IsHtml = true
        });

        await EsperarA(() => emailService.Invocations.Count > 0);

        // Assert
        emailService.Verify(s => s.SendEmailAsync(It.IsAny<EmailMessage>()), Times.Once);

        logger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        await worker.StopAsync(CancellationToken.None);
        worker.Dispose();
    }

    /// <summary>
    /// Espera activa con limite de tiempo: el worker procesa la cola en segundo plano.
    /// </summary>
    private static async Task EsperarA(Func<bool> condicion) {
        var limite = DateTime.UtcNow.AddSeconds(5);

        while (!condicion() && DateTime.UtcNow < limite)
            await Task.Delay(25);
    }
}
