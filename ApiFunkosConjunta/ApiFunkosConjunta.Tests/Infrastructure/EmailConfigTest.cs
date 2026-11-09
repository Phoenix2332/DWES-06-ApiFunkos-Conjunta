using ApiFunkosConjunta.Services.Email;
using ApiFunkosConjunta.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace ApiFunkosConjunta.Tests.Infrastructure;

[TestFixture]
public class EmailConfigTest {
    private static IWebHostEnvironment CrearEntorno(string nombre) {
        var environment = new Mock<IWebHostEnvironment>();

        environment.SetupGet(e => e.EnvironmentName).Returns(nombre);

        return environment.Object;
    }

    [Test]
    public void AddEmail_EnDesarrollo_RegistraElServicioDeEmailEnMemoria() {
        // Arrange
        var services = new ServiceCollection();

        services.AddLogging();

        // Act
        services.AddEmail(CrearEntorno("Development"));

        var provider = services.BuildServiceProvider();

        var descriptor = services.First(s => s.ServiceType == typeof(IEmailService));

        // Assert
        descriptor.ImplementationType.Should().Be<MemoryEmailService>();
        provider.GetService<IEmailService>().Should().BeOfType<MemoryEmailService>();
    }

    [Test]
    public void AddEmail_EnProduccion_RegistraElServicioDeEmailConMailKitYElWorker() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEmail(CrearEntorno("Production"));

        var descriptor = services.First(s => s.ServiceType == typeof(IEmailService));

        var hostedService = services.FirstOrDefault(s =>
            s.ServiceType == typeof(IHostedService)
            && s.ImplementationType == typeof(EmailBackgroundService));

        // Assert
        descriptor.ImplementationType.Should().Be<MailKitEmailService>();
        hostedService.Should().NotBeNull();
    }

    [Test]
    public void AddEmail_RegistraLaColaDeEmailsCompartida() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddEmail(CrearEntorno("Development"));

        var provider = services.BuildServiceProvider();

        // Assert
        provider.GetService<System.Threading.Channels.Channel<EmailMessage>>().Should().NotBeNull();
    }
}
