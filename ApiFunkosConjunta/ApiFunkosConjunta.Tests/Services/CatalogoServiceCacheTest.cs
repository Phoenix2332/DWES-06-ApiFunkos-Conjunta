using System.Text;
using System.Text.Json;
using ApiFunkosConjunta.Common.Json;
using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Dto.Proveedores;
using FluentAssertions;
using MongoDB.Bson;
using Moq;

namespace ApiFunkosConjunta.Tests.Services;

/// <summary>
/// Pruebas de las rutas que devuelven los datos desde la cache de Redis sin consultar
/// los repositorios ni los servicios.
/// </summary>
[TestFixture]
public class CatalogoServiceCacheTest : CatalogoServiceTestBase {
    private const string _categoriaId = "abcdefghijk";
    private static readonly ObjectId _proveedorId = ObjectId.Parse("64b000000000000000000001");

    private static readonly JsonSerializerOptions _jsonOptions = new() {
        Converters = { new ObjectIdJsonConverter() }
    };

    [Test]
    public async Task GetAllCategoriasAsync_ConDatosEnCache_RetornaDatosSinConsultarServicio() {
        // Arrange
        IReadOnlyList<CategoriaDto> categorias = [
            new() { Id = _categoriaId, Nombre = "Videojuegos" }
        ];

        var json = JsonSerializer.Serialize(new { Categorias = categorias, Total = 1 });

        CacheMock
            .Setup(c => c.GetAsync("categorias:1:todas:1:10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(json));

        // Act
        var resultado = await CatalogoService.GetAllCategoriasAsync(null, 1, 10);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Categorias.Should().ContainSingle();
        resultado.Value.Total.Should().Be(1);

        CategoriaServiceMock.Verify(
            x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetAllProveedoresAsync_ConDatosEnCache_RetornaDatosSinConsultarServicio() {
        // Arrange
        IReadOnlyList<ProveedorDto> proveedores = [
            new() {
                Id = _proveedorId,
                Nombre = "Bandai",
                Contacto = "contacto@bandai.com",
                Pais = "Japón"
            }
        ];

        var json = JsonSerializer.Serialize(new { Proveedores = proveedores, Total = 1 }, _jsonOptions);

        CacheMock
            .Setup(c => c.GetAsync("proveedores:1:todos:1:10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(json));

        // Act
        var resultado = await CatalogoService.GetAllProveedoresAsync(null, 1, 10);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Proveedores.Should().ContainSingle();
        resultado.Value.Proveedores[0].Id.Should().Be(_proveedorId);
        resultado.Value.Total.Should().Be(1);

        ProveedorServiceMock.Verify(
            x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetCategoriaByIdAsync_ConDatoEnCache_RetornaLaCategoriaSinConsultarServicio() {
        // Arrange
        var json = JsonSerializer.Serialize(new CategoriaDto { Id = _categoriaId, Nombre = "Videojuegos" });

        CacheMock
            .Setup(c => c.GetAsync($"categoria:{_categoriaId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(json));

        // Act
        var resultado = await CatalogoService.GetCategoriaByIdAsync(_categoriaId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Id.Should().Be(_categoriaId);
        resultado.Value.Nombre.Should().Be("Videojuegos");

        CategoriaServiceMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetProveedorByIdAsync_ConDatoEnCache_RetornaElProveedorSinConsultarServicio() {
        // Arrange
        var json = JsonSerializer.Serialize(
            new ProveedorDto {
                Id = _proveedorId,
                Nombre = "Bandai",
                Contacto = "contacto@bandai.com",
                Pais = "Japón"
            },
            _jsonOptions);

        CacheMock
            .Setup(c => c.GetAsync($"proveedor:{_proveedorId}", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(json));

        // Act
        var resultado = await CatalogoService.GetProveedorByIdAsync(_proveedorId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Id.Should().Be(_proveedorId);
        resultado.Value.Nombre.Should().Be("Bandai");

        ProveedorServiceMock.Verify(
            x => x.GetByIdAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetFunkoByIdAsync_ConDatoEnCache_RetornaElFunkoSinConsultarServicio() {
        // Arrange
        var json = JsonSerializer.Serialize(new FunkoDto {
            Id = 1,
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        });

        CacheMock
            .Setup(c => c.GetAsync("funko:1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(json));

        // Act
        var resultado = await CatalogoService.GetFunkoByIdAsync(1);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Id.Should().Be(1);
        resultado.Value.Nombre.Should().Be("Spider-Man");

        FunkoServiceMock.Verify(
            x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
