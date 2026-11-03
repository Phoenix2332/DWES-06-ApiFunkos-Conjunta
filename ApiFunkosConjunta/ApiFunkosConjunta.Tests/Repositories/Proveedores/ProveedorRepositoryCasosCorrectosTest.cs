using FluentAssertions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ApiFunkosConjunta.Tests.Repositories.Proveedores;

[TestFixture]
public class ProveedorRepositoryCasosCorrectosTest : ProveedorRepositoryTestBase {
    [Test]
    public async Task GetAllAsync_ConProveedores_RetornaProveedoresPaginados() {
        // Arrange
        var proveedores = new[] {
            CreateProveedor("Acción"),
            CreateProveedor("Aventura"),
            CreateProveedor("Deportes")
        };

        await Collection.InsertManyAsync(proveedores);

        // Act
        var result = await Repository.GetAllAsync(null, 1, 2);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Proveedores.Should().HaveCount(2);
        result.Value.Total.Should().Be(3);

        result.Value.Proveedores
            .Select(p => p.Nombre)
            .Should()
            .ContainInOrder("Acción", "Aventura");
    }

    [Test]
    public async Task GetAllAsync_ConFiltro_RetornaProveedoresCoincidentes() {
        // Arrange
        var proveedores = new[] {
            CreateProveedor("Logitech"),
            CreateProveedor("Ikea"),
            CreateProveedor("Logitech España")
        };

        await Collection.InsertManyAsync(proveedores);

        // Act
        var result = await Repository.GetAllAsync("Logitech", 1, 10);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Total.Should().Be(2);

        result.Value.Proveedores
            .Select(p => p.Nombre)
            .Should()
            .Contain(["Logitech", "Logitech España"]);
    }

    [Test]
    public async Task GetByIdAsync_ProveedorExistente_RetornaProveedor() {
        // Arrange
        var proveedor = CreateProveedor();

        await Collection.InsertOneAsync(proveedor);

        // Act
        var result = await Repository.GetByIdAsync(proveedor.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Id.Should().Be(proveedor.Id);
        result.Value.Nombre.Should().Be("Logitech");
        result.Value.Contacto.Should().Be("exportaciones@logitech.com");
        result.Value.Pais.Should().Be("Suiza");
    }

    [Test]
    public async Task AddAsync_ProveedorValido_GuardaProveedorConIdGenerado() {
        // Arrange
        var proveedor = CreateProveedor();

        // Act
        var result = await Repository.AddAsync(proveedor);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().NotBe(ObjectId.Empty);

        var proveedorGuardado = await Collection
            .Find(p => p.Id == result.Value.Id)
            .FirstOrDefaultAsync();

        proveedorGuardado.Should().NotBeNull();
        proveedorGuardado.Nombre.Should().Be("Logitech");
    }

    [Test]
    public async Task UpdateAsync_ProveedorExistente_ActualizaProveedor() {
        // Arrange
        var proveedor = CreateProveedor();

        await Collection.InsertOneAsync(proveedor);

        proveedor.Nombre = "Funko";
        proveedor.Pais = "Estados Unidos";

        // Act
        var result = await Repository.UpdateAsync(proveedor);

        // Assert
        result.IsSuccess.Should().BeTrue();

        var proveedorActualizado = await Collection
            .Find(p => p.Id == proveedor.Id)
            .FirstOrDefaultAsync();

        proveedorActualizado.Should().NotBeNull();
        proveedorActualizado.Nombre.Should().Be("Funko");
        proveedorActualizado.Pais.Should().Be("Estados Unidos");
    }

    [Test]
    public async Task DeleteAsync_ProveedorExistente_EliminaProveedor() {
        // Arrange
        var proveedor = CreateProveedor();

        await Collection.InsertOneAsync(proveedor);

        // Act
        var result = await Repository.DeleteAsync(proveedor);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        var proveedorEliminado = await Collection
            .Find(p => p.Id == proveedor.Id)
            .FirstOrDefaultAsync();

        proveedorEliminado.Should().BeNull();
    }

    [Test]
    public async Task ExistsByNameAsync_ProveedorExistente_RetornaTrue() {
        // Arrange
        var proveedor = CreateProveedor();

        await Collection.InsertOneAsync(proveedor);

        // Act
        var result = await Repository.ExistsByNameAsync("Logitech");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Test]
    public async Task ExistsByNameAsync_ProveedorExistente_NoDistingueMayusculas() {
        // Arrange
        var proveedor = CreateProveedor();

        await Collection.InsertOneAsync(proveedor);

        // Act
        var result = await Repository.ExistsByNameAsync("logitech");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}
