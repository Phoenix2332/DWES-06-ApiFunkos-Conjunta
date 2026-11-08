using System.Text;
using System.Text.Json;
using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson;
using Moq;

namespace ApiFunkosConjunta.Tests.Services;

[TestFixture]
public class CatalogoServiceCasosCorrectosTest : CatalogoServiceTestBase {
    private const string _categoriaId = "abcdefghijk";

    [Test]
    public async Task GetAllCategoriasAsync_ConCategorias_RetornaCategorias() {
        // Arrange
        IReadOnlyList<CategoriaDto> categorias = [
            new() { Id = _categoriaId, Nombre = "Videojuegos" }
        ];

        CategoriaServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>((categorias, 1)));

        // Act
        var resultado = await CatalogoService.GetAllCategoriasAsync(null, 1, 10);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Categorias.Should().HaveCount(1);
        resultado.Value.Total.Should().Be(1);

        CategoriaServiceMock.Verify(x => x.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetCategoriaByIdAsync_ConCategoria_RetornaCategoria() {
        // Arrange
        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await CatalogoService.GetCategoriaByIdAsync(_categoriaId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(categoria);

        CategoriaServiceMock.Verify(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateCategoriaAsync_ConDatosValidos_CreaCategoria() {
        // Arrange
        var dto = new CrearCategoriaDto {
            Nombre = "Videojuegos"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        CategoriaServiceMock
            .Setup(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await CatalogoService.CreateCategoriaAsync(dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(categoria);

        CategoriaServiceMock.Verify(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateCategoriaAsync_ConDatosValidos_ActualizaCategoria() {
        // Arrange
        var dto = new ActualizarCategoriaDto {
            Nombre = "Figuras"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Figuras"
        };

        CategoriaServiceMock
            .Setup(x => x.UpdateAsync(_categoriaId, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await CatalogoService.UpdateCategoriaAsync(
            _categoriaId, dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(categoria);

        CategoriaServiceMock.Verify(x => x.UpdateAsync(_categoriaId, dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteCategoriaAsync_SinFunkosAsociados_EliminaCategoria() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.ExistsByCategoriaIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        CategoriaServiceMock
            .Setup(x => x.DeleteAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteCategoriaAsync(_categoriaId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeTrue();

        FunkoServiceMock.Verify(x => x.ExistsByCategoriaIdAsync(_categoriaId, It.IsAny<CancellationToken>()),
            Times.Once);

        CategoriaServiceMock.Verify(x => x.DeleteAsync(_categoriaId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateCategoriaAsync_ConDatosValidos_InvalidaCacheDeListados() {
        // Arrange
        var dto = new CrearCategoriaDto {
            Nombre = "Videojuegos"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        CategoriaServiceMock
            .Setup(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await CatalogoService.CreateCategoriaAsync(dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(categoria);

        CacheMock.Verify(
            x => x.SetAsync("categorias:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateCategoriaAsync_ConDatosValidos_InvalidaCache() {
        // Arrange
        var dto = new ActualizarCategoriaDto {
            Nombre = "Figuras"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Figuras"
        };

        CategoriaServiceMock
            .Setup(x => x.UpdateAsync(_categoriaId, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await CatalogoService.UpdateCategoriaAsync(_categoriaId, dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(categoria);

        CategoriaServiceMock.Verify(x => x.UpdateAsync(_categoriaId, dto, It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(
            x => x.SetAsync("categorias:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(x => x.RemoveAsync($"categoria:{_categoriaId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteCategoriaAsync_SinFunkosAsociados_InvalidaCache() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.ExistsByCategoriaIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        CategoriaServiceMock
            .Setup(x => x.DeleteAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteCategoriaAsync(_categoriaId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeTrue();

        FunkoServiceMock.Verify(x => x.ExistsByCategoriaIdAsync(_categoriaId, It.IsAny<CancellationToken>()),
            Times.Once);

        CategoriaServiceMock.Verify(x => x.DeleteAsync(_categoriaId, It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(
            x => x.SetAsync("categorias:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(x => x.RemoveAsync($"categoria:{_categoriaId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    private static readonly ObjectId _proveedorId = ObjectId.Parse("64b000000000000000000001");

    [Test]
    public async Task GetAllProveedoresAsync_ConProveedores_RetornaProveedores() {
        // Arrange
        IReadOnlyList<ProveedorDto> proveedores = [
            new() {
                Id = _proveedorId,
                Nombre = "Bandai",
                Contacto = "contacto@bandai.com",
                Pais = "Japón"
            }
        ];

        ProveedorServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>((proveedores, 1)));

        // Act
        var resultado = await CatalogoService.GetAllProveedoresAsync(null, 1, 10);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Proveedores.Should().HaveCount(1);
        resultado.Value.Total.Should().Be(1);

        ProveedorServiceMock.Verify(x => x.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetProveedorByIdAsync_ConProveedor_RetornaProveedor() {
        // Arrange
        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await CatalogoService.GetProveedorByIdAsync(_proveedorId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(proveedor);

        ProveedorServiceMock.Verify(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateProveedorAsync_ConDatosValidos_CreaProveedor() {
        // Arrange
        var dto = new CrearProveedorDto {
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        ProveedorServiceMock
            .Setup(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await CatalogoService.CreateProveedorAsync(dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(proveedor);

        ProveedorServiceMock.Verify(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateProveedorAsync_ConDatosValidos_ActualizaProveedor() {
        // Arrange
        var dto = new ActualizarProveedorDto {
            Nombre = "Bandai actualizado",
            Contacto = "nuevo@bandai.com",
            Pais = "Japón"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai actualizado",
            Contacto = "nuevo@bandai.com",
            Pais = "Japón"
        };

        ProveedorServiceMock
            .Setup(x => x.UpdateAsync(_proveedorId, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await CatalogoService.UpdateProveedorAsync(_proveedorId, dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(proveedor);

        ProveedorServiceMock.Verify(x => x.UpdateAsync(_proveedorId, dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteProveedorAsync_SinFunkosAsociados_EliminaProveedor() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.ExistsByProveedorIdAsync(_proveedorId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        ProveedorServiceMock
            .Setup(x => x.DeleteAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteProveedorAsync(_proveedorId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeTrue();

        FunkoServiceMock.Verify(x => x.ExistsByProveedorIdAsync(_proveedorId.ToString(), It.IsAny<CancellationToken>()),
            Times.Once);

        ProveedorServiceMock.Verify(x => x.DeleteAsync(_proveedorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateProveedorAsync_ConDatosValidos_InvalidaCacheDeListados() {
        // Arrange
        var dto = new CrearProveedorDto {
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        ProveedorServiceMock
            .Setup(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await CatalogoService.CreateProveedorAsync(dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(proveedor);

        ProveedorServiceMock.Verify(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(
            x => x.SetAsync("proveedores:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateProveedorAsync_ConDatosValidos_InvalidaCache() {
        // Arrange
        var dto = new ActualizarProveedorDto {
            Nombre = "Bandai actualizado",
            Contacto = "nuevo@bandai.com",
            Pais = "Japón"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai actualizado",
            Contacto = "nuevo@bandai.com",
            Pais = "Japón"
        };

        ProveedorServiceMock
            .Setup(x => x.UpdateAsync(_proveedorId, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await CatalogoService.UpdateProveedorAsync(_proveedorId, dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(proveedor);

        ProveedorServiceMock.Verify(x => x.UpdateAsync(_proveedorId, dto, It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(
            x => x.SetAsync("proveedores:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(x => x.RemoveAsync($"proveedor:{_proveedorId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteProveedorAsync_SinFunkosAsociados_InvalidaCache() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.ExistsByProveedorIdAsync(_proveedorId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        ProveedorServiceMock
            .Setup(x => x.DeleteAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteProveedorAsync(_proveedorId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeTrue();

        FunkoServiceMock.Verify(x => x.ExistsByProveedorIdAsync(_proveedorId.ToString(), It.IsAny<CancellationToken>()),
            Times.Once);

        ProveedorServiceMock.Verify(x => x.DeleteAsync(_proveedorId, It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(
            x => x.SetAsync("proveedores:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(x => x.RemoveAsync($"proveedor:{_proveedorId}", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllFunkosAsync_ConFunkos_RetornaFunkos() {
        // Arrange
        IReadOnlyList<FunkoDto> funkos = [
            new() {
                Id = 1,
                Nombre = "Spider-Man",
                CategoriaId = _categoriaId,
                ProveedorId = _proveedorId.ToString(),
                Stock = 10,
                Precio = 14.99m,
                Imagen = "imagenes/funkos/spider-man.png"
            }
        ];

        FunkoServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>((funkos, 1)));

        // Act
        var resultado = await CatalogoService.GetAllFunkosAsync(null, 1, 10);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Funkos.Should().HaveCount(1);
        resultado.Value.Total.Should().Be(1);

        FunkoServiceMock.Verify(x => x.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetFunkoByIdAsync_ConFunko_RetornaFunko() {
        // Arrange
        var funko = new FunkoDto {
            Id = 1,
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        FunkoServiceMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await CatalogoService.GetFunkoByIdAsync(1);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(funko);

        FunkoServiceMock.Verify(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task CreateFunkoAsync_ConCategoriaYProveedorExistentes_CreaFunko() {
        // Arrange
        const int UsuarioId = 1;

        var dto = new CrearFunkoDto {
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        var funko = new FunkoDto {
            Id = 1,
            Nombre = dto.Nombre,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            Stock = dto.Stock,
            Precio = dto.Precio,
            Imagen = dto.Imagen
        };

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        FunkoServiceMock
            .Setup(x => x.CreateAsync(dto, UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await CatalogoService.CreateFunkoAsync(dto, UsuarioId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(funko);

        CategoriaServiceMock.Verify(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()), Times.Once);

        ProveedorServiceMock.Verify(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()), Times.Once);

        FunkoServiceMock.Verify(x => x.CreateAsync(dto, UsuarioId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task UpdateFunkoAsync_ConCategoriaYProveedorExistentes_ActualizaFunko() {
        // Arrange
        var dto = new ActualizarFunkoDto {
            Nombre = "Spider-Man actualizado",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 15,
            Precio = 16.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        var funko = new FunkoDto {
            Id = 1,
            Nombre = dto.Nombre,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            Stock = dto.Stock,
            Precio = dto.Precio,
            Imagen = dto.Imagen
        };

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        FunkoServiceMock
            .Setup(x => x.UpdateAsync(1, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await CatalogoService.UpdateFunkoAsync(1, dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeEquivalentTo(funko);

        CategoriaServiceMock.Verify(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()), Times.Once);

        ProveedorServiceMock.Verify(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()), Times.Once);

        FunkoServiceMock.Verify(x => x.UpdateAsync(1, dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteFunkoAsync_ConFunkoExistente_EliminaFunko() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteFunkoAsync(1);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeTrue();

        FunkoServiceMock.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GetAllFunkosAsync_ConDatosEnCache_RetornaDatosSinConsultarServicio() {
        // Arrange
        var funkos = new List<FunkoDto> {
            new() {
                Id = 1,
                Nombre = "Spider-Man"
            }
        };

        var datosCache = new {
            Funkos = funkos,
            Total = 1
        };

        var json = JsonSerializer.Serialize(datosCache);

        CacheMock
            .Setup(c => c.GetAsync("funkos:version", It.IsAny<CancellationToken>()))
            .ReturnsAsync("1"u8.ToArray());

        CacheMock
            .Setup(c => c.GetAsync("funkos:1:todos:1:10", It.IsAny<CancellationToken>()))
            .ReturnsAsync(Encoding.UTF8.GetBytes(json));

        // Act
        var resultado = await CatalogoService.GetAllFunkosAsync(null, 1, 10, CancellationToken.None);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Total.Should().Be(1);
        resultado.Value.Funkos.Should().ContainSingle();

        FunkoServiceMock.Verify(
            s => s.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task CreateFunkoAsync_ConCategoriaYProveedorExistentes_InvalidaCacheDeListados() {
        // Arrange
        const int UsuarioId = 1;

        var dto = new CrearFunkoDto {
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        var funko = new FunkoDto {
            Id = 1,
            Nombre = dto.Nombre,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            Stock = dto.Stock,
            Precio = dto.Precio,
            Imagen = dto.Imagen
        };

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        FunkoServiceMock
            .Setup(x => x.CreateAsync(dto, UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await CatalogoService.CreateFunkoAsync(dto, UsuarioId);

        // Assert
        resultado.IsSuccess.Should().BeTrue();

        CacheMock.Verify(
            x => x.SetAsync("funkos:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task UpdateFunkoAsync_ConCategoriaYProveedorExistentes_InvalidaCache() {
        // Arrange
        var dto = new ActualizarFunkoDto {
            Nombre = "Spider-Man actualizado",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 15,
            Precio = 16.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        var proveedor = new ProveedorDto {
            Id = _proveedorId,
            Nombre = "Bandai",
            Contacto = "contacto@bandai.com",
            Pais = "Japón"
        };

        var funko = new FunkoDto {
            Id = 1,
            Nombre = dto.Nombre,
            CategoriaId = dto.CategoriaId,
            ProveedorId = dto.ProveedorId,
            Stock = dto.Stock,
            Precio = dto.Precio,
            Imagen = dto.Imagen
        };

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        FunkoServiceMock
            .Setup(x => x.UpdateAsync(1, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await CatalogoService.UpdateFunkoAsync(1, dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();

        CacheMock.Verify(
            x => x.SetAsync("funkos:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(x => x.RemoveAsync("funko:1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task DeleteFunkoAsync_ConFunkoExistente_InvalidaCache() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteFunkoAsync(1);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().BeTrue();

        FunkoServiceMock.Verify(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(
            x => x.SetAsync("funkos:version", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);

        CacheMock.Verify(x => x.RemoveAsync("funko:1", It.IsAny<CancellationToken>()), Times.Once);
    }
}