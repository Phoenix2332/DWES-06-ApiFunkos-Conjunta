using ApiFunkosConjunta.Infrastructure;
using ApiFunkosConjunta.Infrastructure.Filters;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiFunkosConjunta.Tests.Infrastructure;

[TestFixture]
public class SwaggerConfigTest {
    private SwaggerGenOptions _options = null!;

    [SetUp]
    public void SetUp() {
        var services = new ServiceCollection();

        services.AddSwagger();

        _options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<SwaggerGenOptions>>()
            .Value;
    }

    [Test]
    public void AddSwagger_RegistraElDocumentoV1ConLaInformacionDeLaApi() {
        // Arrange & Act
        var documento = _options.SwaggerGeneratorOptions.SwaggerDocs["v1"];

        // Assert
        documento.Title.Should().Be("ApiFunkosConjunta API");
        documento.Version.Should().Be("v1.0.0");
        documento.Description.Should().Contain("Funkos");
        documento.Contact.Should().NotBeNull();
    }

    [Test]
    public void AddSwagger_RegistraElEsquemaDeSeguridadBearer() {
        // Arrange & Act
        var esquema = _options.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        esquema.Type.Should().Be(SecuritySchemeType.Http);
        esquema.Scheme.Should().Be("Bearer");
        esquema.BearerFormat.Should().Be("JWT");
        esquema.In.Should().Be(ParameterLocation.Header);
    }

    [Test]
    public void AddSwagger_RegistraElFiltroDeEjemplosPersonalizado() {
        // Arrange & Act
        var filtros = _options.OperationFilterDescriptors;

        // Assert
        filtros.Should().Contain(descriptor => descriptor.Type == typeof(SwaggerExamplesFilter));
    }

    [Test]
    public void AddSwagger_ActivaLosComentariosXmlYLosAtributosDeDocumentacion() {
        // Arrange & Act
        var nombres = _options.OperationFilterDescriptors
            .Concat(_options.SchemaFilterDescriptors)
            .Concat(_options.ParameterFilterDescriptors)
            .Select(descriptor => descriptor.Type.Name)
            .ToList();

        // Assert
        nombres.Should().Contain(nombre => nombre.Contains("XmlComments"));
        nombres.Should().Contain(nombre => nombre.Contains("Annotations"));
    }

    [Test]
    public void AddSwagger_ActivaLosEjemplosDePeticion() {
        // Arrange & Act
        var nombres = _options.OperationFilterDescriptors
            .Select(descriptor => descriptor.Type.Name)
            .ToList();

        // Assert
        nombres.Should().Contain(nombre => nombre.Contains("Examples"));
    }
}
