using ApiFunkosConjunta.Common.Configuration;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Common.Configuration;

[TestFixture]
public class SmtpSettingsTest {
    [Test]
    public void SmtpSettings_SinConfigurar_UsaLosValoresPorDefecto() {
        // Arrange & Act
        var settings = new SmtpSettings();

        // Assert
        settings.Host.Should().Be("localhost");
        settings.Port.Should().Be(587);
        settings.Username.Should().BeNull();
        settings.Password.Should().BeNull();
        settings.FromEmail.Should().Be("noreply@apifunkos.com");
        settings.FromName.Should().Be("ApiFunkos");
        settings.AdminEmail.Should().Be("admin@apifunkos.com");
        settings.Security.Should().Be("Auto");
    }
}
