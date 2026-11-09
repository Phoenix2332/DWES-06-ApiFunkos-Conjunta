using ApiFunkosConjunta.Infrastructure;
using ApiFunkosConjunta.Repositories.Implementations;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services;
using ApiFunkosConjunta.Services.Email;
using ApiFunkosConjunta.Services.Implementations;
using ApiFunkosConjunta.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace ApiFunkosConjunta.Tests.Infrastructure;

[TestFixture]
public class DependencyInjectionTest {
    private static IConfiguration CrearConfiguracion() {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> {
                ["ConnectionStrings:PostgreSQL"] = "Host=localhost;Port=5432;Database=ApiFunkos;Username=postgres;Password=postgres",
                ["ConnectionStrings:Redis"] = "localhost:6379",
                ["MongoDB:ConnectionString"] = "mongodb://admin:admin@localhost:27017/?authSource=admin",
                ["MongoDB:DatabaseName"] = "ApiFunkos",
                ["Jwt:Key"] = "ApiFunkosConjunta_Clave_Segura_De_Pruebas_2026",
                ["Jwt:Issuer"] = "ApiFunkosConjunta",
                ["Jwt:Audience"] = "ApiFunkosConjunta"
            })
            .Build();
    }

    private static IWebHostEnvironment CrearEntorno(string nombre) {
        var environment = new Mock<IWebHostEnvironment>();

        environment.SetupGet(e => e.EnvironmentName).Returns(nombre);

        return environment.Object;
    }

    [Test]
    public void AddApplicationServices_RegistraLosRepositoriosYServicios() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices(CrearConfiguracion(), CrearEntorno("Development"));

        // Assert
        services.Should().Contain(s => s.ServiceType == typeof(ICategoriaRepository)
                                       && s.ImplementationType == typeof(CategoriaRepository));

        services.Should().Contain(s => s.ServiceType == typeof(IProveedorRepository)
                                       && s.ImplementationType == typeof(ProveedorRepository));

        services.Should().Contain(s => s.ServiceType == typeof(IFunkoRepository)
                                       && s.ImplementationType == typeof(FunkoRepository));

        services.Should().Contain(s => s.ServiceType == typeof(ICategoriaService)
                                       && s.ImplementationType == typeof(CategoriaService));

        services.Should().Contain(s => s.ServiceType == typeof(IProveedorService)
                                       && s.ImplementationType == typeof(ProveedorService));

        services.Should().Contain(s => s.ServiceType == typeof(IFunkoService)
                                       && s.ImplementationType == typeof(FunkoService));

        services.Should().Contain(s => s.ServiceType == typeof(ICatalogoService)
                                       && s.ImplementationType == typeof(CatalogoService));

        services.Should().Contain(s => s.ServiceType == typeof(IAuthService)
                                       && s.ImplementationType == typeof(AuthService));
    }

    [Test]
    public void AddApplicationServices_RegistraLaAutenticacionYLaDocumentacion() {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddApplicationServices(CrearConfiguracion(), CrearEntorno("Development"));

        // Assert
        services.Should().Contain(s => s.ServiceType == typeof(IEmailService));
        services.Should().Contain(s => s.ServiceType == typeof(Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider));
    }

    [Test]
    public void AddApplicationServices_ConfiguraLaValidacionDelTokenJwt() {
        // Arrange
        var services = new ServiceCollection();

        services.AddLogging();

        services.AddApplicationServices(CrearConfiguracion(), CrearEntorno("Development"));

        var provider = services.BuildServiceProvider();

        // Act
        var opciones = provider
            .GetRequiredService<IOptionsMonitor<JwtBearerOptions>>()
            .Get(JwtBearerDefaults.AuthenticationScheme);

        // Assert
        opciones.TokenValidationParameters.ValidIssuer.Should().Be("ApiFunkosConjunta");
        opciones.TokenValidationParameters.ValidAudience.Should().Be("ApiFunkosConjunta");
        opciones.TokenValidationParameters.ValidateIssuerSigningKey.Should().BeTrue();
    }
}
