using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Funkos;

[TestFixture]
public class FunkoControllerCasosIncorrectosTest : FunkoControllerTestBase {
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
            s => s.GetAllFunkosAsync(
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
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
            s => s.GetAllFunkosAsync(
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetAll_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        Service
            .Setup(s => s.GetAllFunkosAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>(ServiceError.GetAllError()));

        // Act
        var resultado = await Controller.GetAll(null, 1, 10, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task GetById_FunkoInexistente_RetornaNotFound() {
        // Arrange
        const int Id = 999;

        Service
            .Setup(s => s.GetFunkoByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(ServiceError.NotFoundError(Id.ToString())));

        // Act
        var resultado = await Controller.GetById(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task GetById_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        const int Id = 1;

        Service
            .Setup(s => s.GetFunkoByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(ServiceError.GetByIdError(Id.ToString())));

        // Act
        var resultado = await Controller.GetById(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Create_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        var request = CreateCreateRequest();

        Service
            .Setup(s => s.CreateFunkoAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(ServiceError.CreateError()));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Update_FunkoInexistente_RetornaNotFound() {
        // Arrange
        const int Id = 999;
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateFunkoAsync(Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(ServiceError.NotFoundError(Id.ToString())));

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task Update_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        const int Id = 1;
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateFunkoAsync(Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(ServiceError.UpdateError(Id.ToString())));

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }

    [Test]
    public async Task Delete_FunkoInexistente_RetornaNotFound() {
        // Arrange
        const int Id = 999;

        Service
            .Setup(s => s.DeleteFunkoAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(ServiceError.NotFoundError(Id.ToString())));

        // Act
        var resultado = await Controller.Delete(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task Delete_ErrorServicio_RetornaInternalServerError() {
        // Arrange
        const int Id = 1;

        Service
            .Setup(s => s.DeleteFunkoAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(ServiceError.DeleteError(Id.ToString())));

        // Act
        var resultado = await Controller.Delete(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<ObjectResult>();

        var objectResult = (ObjectResult)resultado;
        objectResult.StatusCode.Should().Be(500);
    }
}