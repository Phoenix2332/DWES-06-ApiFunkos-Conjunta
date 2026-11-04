using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Moq;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Tests.Services.Proveedores;

[TestFixture]
public class ProveedorServiceCasosIncorrectosTest : ProveedorServiceTestBase {
    [Test]
    public async Task GetByIdAsync_ProveedorInexistente_RetornaNull() {
        // Arrange
        var id = ObjectId.GenerateNewId();

        Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(null));

        // Act
        var result = await Service.GetByIdAsync(id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorServiceErrors.NotFoundError>();
    }

    [Test]
    public async Task UpdateAsync_ProveedorInexistente_NoActualizaProveedor() {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(null));

        // Act
        var result = await Service.UpdateAsync(id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorServiceErrors.NotFoundError>();

        Repository.Verify(r => r.UpdateAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_ProveedorInexistente_NoEliminaProveedor() {
        // Arrange
        var id = ObjectId.GenerateNewId();

        Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(null));

        // Act
        var result = await Service.DeleteAsync(id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorServiceErrors.NotFoundError>();

        Repository.Verify(r => r.DeleteAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_NombreDuplicado_NoCreaProveedor() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.ExistsByNameAsync("Logitech", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorServiceErrors.DuplicateNameError>();

        Repository.Verify(r => r.AddAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_NombreDuplicado_NoActualizaProveedor() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(proveedor));

        Repository
            .Setup(r => r.ExistsByNameAsync("Funko", proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.UpdateAsync(proveedor.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorServiceErrors.DuplicateNameError>();

        Repository.Verify(r => r.UpdateAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_ErrorAlComprobarNombre_PropagaErrorRepositorio() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.ExistsByNameAsync("Logitech", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(ProveedorRepositoryError.ExistsByNameError()));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.ExistsByNameError>();

        Repository.Verify(r => r.AddAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task GetByIdAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        var id = ObjectId.GenerateNewId();

        Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Proveedor?, DomainErrors>(ProveedorRepositoryError.GetByIdError()));

        // Act
        var result = await Service.GetByIdAsync(id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.GetByIdError>();
    }

    [Test]
    public async Task DeleteAsync_ErrorAlEliminar_PropagaErrorRepositorio() {
        // Arrange
        var proveedor = CreateProveedor();

        Repository
            .Setup(r => r.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(proveedor));

        Repository
            .Setup(r => r.DeleteAsync(proveedor, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(ProveedorRepositoryError.DeletedError()));

        // Act
        var result = await Service.DeleteAsync(proveedor.Id);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.DeletedError>();
    }

    [Test]
    public async Task GetAllAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        Repository
            .Setup(r => r.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Failure<(IReadOnlyList<Proveedor> Proveedores, int Total), DomainErrors>(
                    ProveedorRepositoryError.GetAllError()));

        // Act
        var result = await Service.GetAllAsync(null, 1, 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.GetAllError>();
    }

    [Test]
    public async Task CreateAsync_ErrorAlCrear_PropagaErrorRepositorio() {
        // Arrange
        var request = CreateRequest();

        Repository
            .Setup(r => r.ExistsByNameAsync("Logitech", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Proveedor, DomainErrors>(ProveedorRepositoryError.CreationError()));

        // Act
        var result = await Service.CreateAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.CreationError>();
    }

    [Test]
    public async Task UpdateAsync_ErrorAlConsultarRepositorio_PropagaError() {
        // Arrange
        var id = ObjectId.GenerateNewId();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Proveedor?, DomainErrors>(ProveedorRepositoryError.GetByIdError()));

        // Act
        var result = await Service.UpdateAsync(id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.GetByIdError>();

        Repository.Verify(
            r => r.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<ObjectId?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task UpdateAsync_ErrorAlComprobarNombre_PropagaErrorRepositorio() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(proveedor));

        Repository
            .Setup(r => r.ExistsByNameAsync("Funko", proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<bool, DomainErrors>(ProveedorRepositoryError.ExistsByNameError()));

        // Act
        var result = await Service.UpdateAsync(proveedor.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.ExistsByNameError>();

        Repository.Verify(r => r.UpdateAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_ErrorAlActualizar_PropagaErrorRepositorio() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Proveedor?, DomainErrors>(proveedor));

        Repository
            .Setup(r => r.ExistsByNameAsync("Funko", proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(false));

        Repository
            .Setup(r => r.UpdateAsync(It.IsAny<Proveedor>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<Proveedor, DomainErrors>(ProveedorRepositoryError.UpdatedError()));

        // Act
        var result = await Service.UpdateAsync(proveedor.Id, request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.UpdatedError>();
    }
}
