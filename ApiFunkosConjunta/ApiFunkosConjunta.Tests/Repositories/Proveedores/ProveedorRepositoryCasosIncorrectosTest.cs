using ApiFunkosConjunta.Errors;
using FluentAssertions;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Tests.Repositories.Proveedores;

[TestFixture]
public class ProveedorRepositoryCasosIncorrectosTest : ProveedorRepositoryTestBase {
    [Test]
    public async Task GetByIdAsync_ProveedorInexistente_RetornaNull() {
        // Arrange
        var id = ObjectId.GenerateNewId();

        // Act
        var result = await Repository.GetByIdAsync(id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_SinProveedoresCoincidentes_RetornaListaVacia() {
        // Arrange
        await Collection.InsertOneAsync(CreateProveedor());

        // Act
        var result = await Repository.GetAllAsync("Ikea", 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Proveedores.Should().BeEmpty();
        result.Value.Total.Should().Be(0);
    }

    [Test]
    public async Task ExistsByNameAsync_ProveedorInexistente_RetornaFalse() {
        // Act
        var result = await Repository.ExistsByNameAsync("Ikea");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Test]
    public async Task ExistsByNameAsync_ExcluyendoProveedorExistente_RetornaFalse() {
        // Arrange
        var proveedor = CreateProveedor();

        await Collection.InsertOneAsync(proveedor);

        // Act
        var result = await Repository.ExistsByNameAsync("Logitech", proveedor.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeFalse();
    }

    [Test]
    public async Task GetAllAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var repository = CreateFailingRepository();

        // Act
        var result = await repository.GetAllAsync(null, 1, 10);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.GetAllError>();
    }

    [Test]
    public async Task GetByIdAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var repository = CreateFailingRepository();

        // Act
        var result = await repository.GetByIdAsync(ObjectId.GenerateNewId());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.GetByIdError>();
    }

    [Test]
    public async Task AddAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var repository = CreateFailingRepository();
        var proveedor = CreateProveedor();

        // Act
        var result = await repository.AddAsync(proveedor);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.CreationError>();
    }

    [Test]
    public async Task UpdateAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var repository = CreateFailingRepository();
        var proveedor = CreateProveedor();

        // Act
        var result = await repository.UpdateAsync(proveedor);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.UpdatedError>();
    }

    [Test]
    public async Task DeleteAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var repository = CreateFailingRepository();
        var proveedor = CreateProveedor();

        // Act
        var result = await repository.DeleteAsync(proveedor);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.DeletedError>();
    }

    [Test]
    public async Task ExistsByNameAsync_ErrorEnRepositorio_RetornaError() {
        // Arrange
        var repository = CreateFailingRepository();

        // Act
        var result = await repository.ExistsByNameAsync("Logitech");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.ExistsByNameError>();
    }

    [Test]
    public async Task UpdateAsync_ProveedorInexistente_RetornaError() {
        // Arrange
        var proveedor = CreateProveedor();

        // Act
        var result = await Repository.UpdateAsync(proveedor);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<ProveedorRepositoryErrors.UpdatedError>();
    }
}