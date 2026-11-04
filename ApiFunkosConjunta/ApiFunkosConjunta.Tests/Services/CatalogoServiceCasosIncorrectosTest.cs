using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using MongoDB.Bson;
using Moq;

namespace ApiFunkosConjunta.Tests.Services;

[TestFixture]
public class CatalogoServiceCasosIncorrectosTest : CatalogoServiceTestBase {
    private const string _categoriaId = "abcdefghijk";
    private static readonly ObjectId _proveedorId = ObjectId.Parse("64b000000000000000000001");

    [Test]
    public async Task DeleteCategoriaAsync_ConFunkosAsociados_RetornaError() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.ExistsByCategoriaIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteCategoriaAsync(_categoriaId);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<CatalogoServiceErrors.CategoriaConFunkosError>();

        CategoriaServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteCategoriaAsync_ErrorAlComprobarFunkos_RetornaError() {
        // Arrange
        var error = RepositoryError.GetAllError();

        FunkoServiceMock
            .Setup(x => x.ExistsByCategoriaIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.DeleteCategoriaAsync(_categoriaId);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);

        CategoriaServiceMock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteProveedorAsync_ConFunkosAsociados_RetornaError() {
        // Arrange
        FunkoServiceMock
            .Setup(x => x.ExistsByProveedorIdAsync(_proveedorId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await CatalogoService.DeleteProveedorAsync(_proveedorId);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<CatalogoServiceErrors.ProveedorConFunkosError>();

        ProveedorServiceMock.Verify(x => x.DeleteAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task DeleteProveedorAsync_ErrorAlComprobarFunkos_RetornaError() {
        // Arrange
        var error = RepositoryError.GetAllError();

        FunkoServiceMock
            .Setup(x => x.ExistsByProveedorIdAsync(_proveedorId.ToString(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.DeleteProveedorAsync(_proveedorId);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);

        ProveedorServiceMock.Verify(x => x.DeleteAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task CreateFunkoAsync_CategoriaInexistente_RetornaError() {
        // Arrange
        var dto = new CrearFunkoDto {
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var error = ServiceError.NotFoundError(_categoriaId);

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.CreateFunkoAsync(dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);

        ProveedorServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()),
            Times.Never);

        FunkoServiceMock.Verify(x => x.CreateAsync(It.IsAny<CrearFunkoDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task CreateFunkoAsync_ProveedorIdInvalido_RetornaError() {
        // Arrange
        var dto = new CrearFunkoDto {
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = "identificador-invalido",
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await CatalogoService.CreateFunkoAsync(dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();

        ProveedorServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()),
            Times.Never);

        FunkoServiceMock.Verify(x => x.CreateAsync(It.IsAny<CrearFunkoDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task CreateFunkoAsync_ProveedorInexistente_RetornaError() {
        // Arrange
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

        var error = ProveedorServiceError.NotFoundError(_proveedorId.ToString());

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.CreateFunkoAsync(dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);

        FunkoServiceMock.Verify(x => x.CreateAsync(It.IsAny<CrearFunkoDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task UpdateFunkoAsync_CategoriaInexistente_RetornaError() {
        // Arrange
        var dto = new ActualizarFunkoDto {
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = _proveedorId.ToString(),
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var error = ServiceError.NotFoundError(_categoriaId);

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.UpdateFunkoAsync(1, dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);

        ProveedorServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()),
            Times.Never);

        FunkoServiceMock.Verify(
            x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<ActualizarFunkoDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task UpdateFunkoAsync_ProveedorInexistente_RetornaError() {
        // Arrange
        var dto = new ActualizarFunkoDto {
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

        var error = ProveedorServiceError.NotFoundError(_proveedorId.ToString());

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.UpdateFunkoAsync(1, dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);

        FunkoServiceMock.Verify(
            x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<ActualizarFunkoDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task UpdateFunkoAsync_ProveedorIdInvalido_RetornaError() {
        // Arrange
        var dto = new ActualizarFunkoDto {
            Nombre = "Spider-Man",
            CategoriaId = _categoriaId,
            ProveedorId = "identificador-invalido",
            Stock = 10,
            Precio = 14.99m,
            Imagen = "imagenes/funkos/spider-man.png"
        };

        var categoria = new CategoriaDto {
            Id = _categoriaId,
            Nombre = "Videojuegos"
        };

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await CatalogoService.UpdateFunkoAsync(1, dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();

        ProveedorServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()),
            Times.Never);

        FunkoServiceMock.Verify(
            x => x.UpdateAsync(It.IsAny<int>(), It.IsAny<ActualizarFunkoDto>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task CreateFunkoAsync_ErrorAlCrear_RetornaError() {
        // Arrange
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

        var error = ServiceError.CreateError();

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        FunkoServiceMock
            .Setup(x => x.CreateAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.CreateFunkoAsync(dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);
    }

    [Test]
    public async Task UpdateFunkoAsync_ErrorAlActualizar_RetornaError() {
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

        var error = ServiceError.UpdateError("1");

        CategoriaServiceMock
            .Setup(x => x.GetByIdAsync(_categoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        ProveedorServiceMock
            .Setup(x => x.GetByIdAsync(_proveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        FunkoServiceMock
            .Setup(x => x.UpdateAsync(1, dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.UpdateFunkoAsync(1, dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);
    }

    [Test]
    public async Task GetAllFunkosAsync_ErrorAlObtener_RetornaError() {
        // Arrange
        var error = RepositoryError.GetAllError();

        FunkoServiceMock
            .Setup(x => x.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.GetAllFunkosAsync(null, 1, 10);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);
    }

    [Test]
    public async Task GetFunkoByIdAsync_ErrorAlObtener_RetornaError() {
        // Arrange
        var error = ServiceError.GetByIdError("1");

        FunkoServiceMock
            .Setup(x => x.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.GetFunkoByIdAsync(1);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);
    }

    [Test]
    public async Task DeleteFunkoAsync_ErrorAlEliminar_RetornaError() {
        // Arrange
        var error = ServiceError.DeleteError("1");

        FunkoServiceMock
            .Setup(x => x.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(error));

        // Act
        var resultado = await CatalogoService.DeleteFunkoAsync(1);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().Be(error);
    }
}