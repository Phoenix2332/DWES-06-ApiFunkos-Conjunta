using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Tests.Controllers.Proveedores;

[TestFixture]
public class ProveedorControllerCasosIncorrectosTest : ProveedorControllerTestBase {
    [Test]
    public async Task GetAll_PaginacionInvalida_RetornaBadRequest() {
        // Arrange
        const int Pagina = 0;
        const int TamanoPagina = 10;

        // Act
        var resultado = await Controller.GetAll(null, Pagina, TamanoPagina, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();

        Service.Verify(
            s => s.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetAll_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        Service
            .Setup(s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>(
                    ProveedorServiceError.GetAllError()));

        // Act
        var resultado = await Controller.GetAll(null, 1, 10, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task GetById_IdInvalido_RetornaBadRequest() {
        // Arrange
        const string Id = "no-es-un-objectid";

        // Act
        var resultado = await Controller.GetById(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();

        Service.Verify(s => s.GetByIdAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetById_ProveedorInexistente_RetornaNotFound() {
        // Arrange
        var proveedor = CreateProveedor();

        Service
            .Setup(s => s.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(
                ProveedorServiceError.NotFoundError(proveedor.Id.ToString())));

        // Act
        var resultado = await Controller.GetById(proveedor.Id.ToString(), CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task GetById_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        var proveedor = CreateProveedor();

        Service
            .Setup(s => s.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(
                ProveedorServiceError.GetByIdError(proveedor.Id.ToString())));

        // Act
        var resultado = await Controller.GetById(proveedor.Id.ToString(), CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Create_NombreDuplicado_RetornaConflict() {
        // Arrange
        var request = CreateCreateRequest();

        Service
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(
                ProveedorServiceError.DuplicateNameError(request.Nombre)));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ConflictObjectResult>();
    }

    [Test]
    public async Task Create_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        var request = CreateCreateRequest();

        Service
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(ProveedorServiceError.CreateError()));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Update_IdInvalido_RetornaBadRequest() {
        // Arrange
        const string Id = "no-es-un-objectid";
        var request = CreateUpdateRequest();

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();

        Service.Verify(s => s.UpdateAsync(It.IsAny<ObjectId>(), It.IsAny<ActualizarProveedorDto>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Update_ProveedorInexistente_RetornaNotFound() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateAsync(proveedor.Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(
                ProveedorServiceError.NotFoundError(proveedor.Id.ToString())));

        // Act
        var resultado = await Controller.Update(proveedor.Id.ToString(), request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task Update_NombreDuplicado_RetornaConflict() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateAsync(proveedor.Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(
                ProveedorServiceError.DuplicateNameError(request.Nombre)));

        // Act
        var resultado = await Controller.Update(proveedor.Id.ToString(), request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ConflictObjectResult>();
    }

    [Test]
    public async Task Update_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateAsync(proveedor.Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProveedorDto, DomainErrors>(
                ProveedorServiceError.UpdateError(proveedor.Id.ToString())));

        // Act
        var resultado = await Controller.Update(proveedor.Id.ToString(), request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Delete_IdInvalido_RetornaBadRequest() {
        // Arrange
        const string Id = "no-es-un-objectid";

        // Act
        var resultado = await Controller.Delete(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();

        Service.Verify(s => s.DeleteAsync(It.IsAny<ObjectId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task Delete_ProveedorInexistente_RetornaNotFound() {
        // Arrange
        var proveedor = CreateProveedor();

        Service
            .Setup(s => s.DeleteAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(
                ProveedorServiceError.NotFoundError(proveedor.Id.ToString())));

        // Act
        var resultado = await Controller.Delete(proveedor.Id.ToString(), CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task Delete_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        var proveedor = CreateProveedor();

        Service
            .Setup(s => s.DeleteAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(
                ProveedorServiceError.DeleteError(proveedor.Id.ToString())));

        // Act
        var resultado = await Controller.Delete(proveedor.Id.ToString(), CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task GetAll_TamanoPaginaSuperiorAlMaximo_RetornaBadRequest() {
        // Arrange
        const int Pagina = 1;
        const int TamanoPagina = 101;

        // Act
        var resultado = await Controller.GetAll(null, Pagina, TamanoPagina, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<BadRequestObjectResult>();

        Service.Verify(
            s => s.GetAllAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
