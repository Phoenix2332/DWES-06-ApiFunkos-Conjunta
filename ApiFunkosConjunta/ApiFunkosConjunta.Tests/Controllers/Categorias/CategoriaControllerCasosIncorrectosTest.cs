using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Categorias;

[TestFixture]
public class CategoriaControllerCasosIncorrectosTest : CategoriaControllerTestBase {
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
                Result.Failure<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>(
                    ServiceError.GetAllError()));

        // Act
        var resultado = await Controller.GetAll(null, 1, 10, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task GetById_CategoriaInexistente_RetornaNotFound() {
        // Arrange
        const string Id = "abcdefghijk";

        Service
            .Setup(s => s.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(ServiceError.NotFoundError(Id)));

        // Act
        var resultado = await Controller.GetById(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task GetById_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        const string Id = "abcdefghijk";

        Service
            .Setup(s => s.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(ServiceError.GetByIdError(Id)));

        // Act
        var resultado = await Controller.GetById(Id, CancellationToken.None);

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
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(ServiceError.DuplicateNameError(request.Nombre)));

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
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(ServiceError.CreateError()));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Update_CategoriaInexistente_RetornaNotFound() {
        // Arrange
        const string Id = "abcdefghijk";
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateAsync(Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(ServiceError.NotFoundError(Id)));

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task Update_NombreDuplicado_RetornaConflict() {
        // Arrange
        const string Id = "abcdefghijk";
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateAsync(Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(ServiceError.DuplicateNameError(request.Nombre)));

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ConflictObjectResult>();
    }

    [Test]
    public async Task Update_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        const string Id = "abcdefghijk";
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateAsync(Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<CategoriaDto, DomainErrors>(ServiceError.UpdateError(Id)));

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Delete_CategoriaInexistente_RetornaNotFound() {
        // Arrange
        const string Id = "abcdefghijk";

        Service
            .Setup(s => s.DeleteAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(ServiceError.NotFoundError(Id)));

        // Act
        var resultado = await Controller.Delete(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task Delete_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        const string Id = "abcdefghijk";

        Service
            .Setup(s => s.DeleteAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(ServiceError.DeleteError(Id)));

        // Act
        var resultado = await Controller.Delete(Id, CancellationToken.None);

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