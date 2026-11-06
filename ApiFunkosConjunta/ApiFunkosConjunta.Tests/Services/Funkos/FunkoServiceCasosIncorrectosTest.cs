using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Services.Email;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Funkos;

[TestFixture]
public class FunkoServiceCasosIncorrectosTest : FunkoServiceTestBase {
    [Test]
    public async Task GetByIdAsync_FunkoInexistente_RetornaErrorNotFound() {
        // Arrange
        const int Id = 999;

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(null));

        // Act
        var result = await Service.GetByIdAsync(Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.NotFoundError>();
    }

    [Test]
    public async Task UpdateAsync_FunkoInexistente_NoActualizaFunko() {
        // Arrange
        const int Id = 999;
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(null));

        // Act
        var result = await Service.UpdateAsync(Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.NotFoundError>();

        Repository.Verify(r => r.UpdateAsync(
                It.IsAny<Funko>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task DeleteAsync_FunkoInexistente_NoEliminaFunko() {
        // Arrange
        const int Id = 999;

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(null));

        // Act
        var result = await Service.DeleteAsync(Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ServiceErrors.NotFoundError>();

        Repository.Verify(r => r.DeleteAsync(
                It.IsAny<Funko>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task GetByIdAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        const int Id = 1;

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Funko?, DomainErrors>(RepositoryError.GetByIdError()));

        // Act
        var result = await Service.GetByIdAsync(Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetByIdError>();
    }

    [Test]
    public async Task GetAllAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        Repository
            .Setup(r => r.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<(IReadOnlyList<Funko> Funkos, int Total), DomainErrors>(RepositoryError.GetAllError()));

        // Act
        var result = await Service.GetAllAsync(null, 1, 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetAllError>();
    }

    [Test]
    public async Task CreateAsync_ErrorAlCrear_PropagaErrorRepositorio() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Funko, DomainErrors>(RepositoryError.CreationError()));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.CreationError>();
    }

    [Test]
    public async Task CreateAsync_ErrorAlCrear_NoEncolaElAvisoAlAdmin() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Funko, DomainErrors>(RepositoryError.CreationError()));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();

        EmailService.Verify(e => e.EnqueueEmailAsync(It.IsAny<EmailMessage>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        const int Id = 1;
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Funko?, DomainErrors>(RepositoryError.GetByIdError()));

        // Act
        var result = await Service.UpdateAsync(Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetByIdError>();

        Repository.Verify(r => r.UpdateAsync(
                It.IsAny<Funko>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task UpdateAsync_ErrorAlActualizar_PropagaErrorRepositorio() {
        // Arrange
        var funko = CreateFunko();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.UpdateAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Funko, DomainErrors>(RepositoryError.UpdatedError()));

        // Act
        var result = await Service.UpdateAsync(funko.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.UpdatedError>();
    }

    [Test]
    public async Task DeleteAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        const int Id = 1;

        Repository
            .Setup(r => r.GetByIdAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Funko?, DomainErrors>(RepositoryError.GetByIdError()));

        // Act
        var result = await Service.DeleteAsync(Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetByIdError>();

        Repository.Verify(r => r.DeleteAsync(
                It.IsAny<Funko>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task DeleteAsync_ErrorAlEliminar_PropagaErrorRepositorio() {
        // Arrange
        var funko = CreateFunko();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.DeleteAsync(funko, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(RepositoryError.DeletedError()));

        // Act
        var result = await Service.DeleteAsync(funko.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.DeletedError>();
    }

    [Test]
    public async Task ExistsByCategoriaIdAsync_ErrorEnRepositorio_PropagaError() {
        // Arrange
        const string CategoriaId = "abcdefghijk";

        Repository
            .Setup(r => r.ExistsByCategoriaIdAsync(CategoriaId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(RepositoryError.GetAllError()));

        // Act
        var result = await Service.ExistsByCategoriaIdAsync(CategoriaId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetAllError>();
    }

    [Test]
    public async Task ExistsByProveedorIdAsync_ErrorEnRepositorio_PropagaError() {
        // Arrange
        const string ProveedorId = "64b000000000000000000001";

        Repository
            .Setup(r => r.ExistsByProveedorIdAsync(ProveedorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(RepositoryError.GetAllError()));

        // Act
        var result = await Service.ExistsByProveedorIdAsync(ProveedorId);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<RepositoryErrors.GetAllError>();
    }
}