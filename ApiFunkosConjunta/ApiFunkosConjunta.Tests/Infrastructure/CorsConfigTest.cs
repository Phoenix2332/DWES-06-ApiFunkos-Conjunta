using ApiFunkosConjunta.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ApiFunkosConjunta.Tests.Infrastructure;

[TestFixture]
public class CorsConfigTest {
    private static IConfiguration CrearConfiguracion(params string[] origenes) {
        var valores = new Dictionary<string, string?>();

        for (var i = 0; i < origenes.Length; i++)
            valores[$"Cors:AllowedOrigins:{i}"] = origenes[i];

        return new ConfigurationBuilder()
            .AddInMemoryCollection(valores)
            .Build();
    }

    [Test]
    public void AddCorsPolicy_EnDesarrollo_RegistraLaPoliticaPermisiva() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCorsPolicy(CrearConfiguracion(), isDevelopment: true);

        var opciones = services.BuildServiceProvider()
            .GetRequiredService<IOptions<CorsOptions>>()
            .Value;

        var politica = opciones.GetPolicy(CorsConfig.AllowAllPolicy);

        // Assert
        politica.Should().NotBeNull();
        politica!.Origins.Should().Contain("*");
        politica.AllowAnyHeader.Should().BeTrue();
        politica.AllowAnyMethod.Should().BeTrue();
    }

    [Test]
    public void AddCorsPolicy_EnProduccion_RegistraSoloLosOrigenesConfigurados() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCorsPolicy(
            CrearConfiguracion("https://mi-frontend.com", "https://admin.mi-frontend.com"),
            isDevelopment: false);

        var opciones = services.BuildServiceProvider()
            .GetRequiredService<IOptions<CorsOptions>>()
            .Value;

        var politica = opciones.GetPolicy(CorsConfig.ProductionPolicy);

        // Assert
        politica.Should().NotBeNull();
        politica!.Origins.Should().BeEquivalentTo("https://mi-frontend.com", "https://admin.mi-frontend.com");
        politica.SupportsCredentials.Should().BeTrue();
    }

    [Test]
    public void AddCorsPolicy_EnProduccion_SinOrigenesConfigurados_LanzaExcepcion() {
        // Arrange
        var services = new ServiceCollection();

        services.AddCorsPolicy(CrearConfiguracion(), isDevelopment: false);

        var provider = services.BuildServiceProvider();

        // Act
        var act = () => provider.GetRequiredService<IOptions<CorsOptions>>().Value;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cors:AllowedOrigins*");
    }

    [Test]
    public void UseCorsPolicy_AplicaLaPoliticaAlPipeline() {
        // Arrange
        var services = new ServiceCollection();

        services.AddLogging();
        services.AddCorsPolicy(CrearConfiguracion("https://mi-frontend.com"), isDevelopment: false);

        var provider = services.BuildServiceProvider();

        var app = new ApplicationBuilder(provider);

        // Act
        var act = () => app.UseCorsPolicy(isDevelopment: false);

        // Assert
        act.Should().NotThrow();
    }
}
