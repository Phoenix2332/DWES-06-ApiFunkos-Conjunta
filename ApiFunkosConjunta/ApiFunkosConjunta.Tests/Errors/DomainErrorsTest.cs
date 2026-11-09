using ApiFunkosConjunta.Errors;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Errors;

[TestFixture]
public class DomainErrorsTest {
    [Test]
    public void ApiErrors_Constructor_CreaErrorCorrectamente() {
        // Arrange
        const string Mensaje = "Error de API.";

        // Act
        var error = new ApiErrors(Mensaje);

        // Assert
        error.Message.Should().Be(Mensaje);
    }

    [Test]
    public void RepositoryError_Fabricas_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            RepositoryError.CreationError(),
            RepositoryError.UpdatedError(),
            RepositoryError.DeletedError(),
            RepositoryError.GetAllError(),
            RepositoryError.GetByIdError(),
            RepositoryError.ExistsByNameError()
        ];

        // Act & Assert
        errores.Should().HaveCount(6);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void RepositoryErrors_Constructores_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            new RepositoryErrors.CreationError(),
            new RepositoryErrors.UpdatedError(),
            new RepositoryErrors.DeletedError(),
            new RepositoryErrors.GetAllError(),
            new RepositoryErrors.GetByIdError(),
            new RepositoryErrors.ExistsByNameError()
        ];

        // Act & Assert
        errores.Should().HaveCount(6);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void ServiceErrors_Constructores_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            new ServiceErrors.NotFoundError("abcdefghijk"),
            new ServiceErrors.GetAllError(),
            new ServiceErrors.GetByIdError("abcdefghijk"),
            new ServiceErrors.CreateError(),
            new ServiceErrors.DuplicateNameError("Videojuegos"),
            new ServiceErrors.UpdateError("abcdefghijk"),
            new ServiceErrors.DeleteError("abcdefghijk")
        ];

        // Act & Assert
        errores.Should().HaveCount(7);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void ServiceError_Fabricas_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            ServiceError.NotFoundError("abcdefghijk"),
            ServiceError.GetAllError(),
            ServiceError.GetByIdError("abcdefghijk"),
            ServiceError.CreateError(),
            ServiceError.DuplicateNameError("Videojuegos"),
            ServiceError.UpdateError("abcdefghijk"),
            ServiceError.DeleteError("abcdefghijk")
        ];

        // Act & Assert
        errores.Should().HaveCount(7);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void ProveedorServiceErrors_Constructores_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            new ProveedorServiceErrors.NotFoundError("64b000000000000000000001"),
            new ProveedorServiceErrors.GetAllError(),
            new ProveedorServiceErrors.GetByIdError("64b000000000000000000001"),
            new ProveedorServiceErrors.CreateError(),
            new ProveedorServiceErrors.DuplicateNameError("Bandai"),
            new ProveedorServiceErrors.UpdateError("64b000000000000000000001"),
            new ProveedorServiceErrors.DeleteError("64b000000000000000000001")
        ];

        // Act & Assert
        errores.Should().HaveCount(7);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void ProveedorServiceError_Fabricas_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            ProveedorServiceError.NotFoundError("64b000000000000000000001"),
            ProveedorServiceError.GetAllError(),
            ProveedorServiceError.GetByIdError("64b000000000000000000001"),
            ProveedorServiceError.CreateError(),
            ProveedorServiceError.DuplicateNameError("Bandai"),
            ProveedorServiceError.UpdateError("64b000000000000000000001"),
            ProveedorServiceError.DeleteError("64b000000000000000000001")
        ];

        // Act & Assert
        errores.Should().HaveCount(7);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void ProveedorRepositoryErrors_Constructores_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            new ProveedorRepositoryErrors.CreationError(),
            new ProveedorRepositoryErrors.UpdatedError(),
            new ProveedorRepositoryErrors.DeletedError(),
            new ProveedorRepositoryErrors.GetAllError(),
            new ProveedorRepositoryErrors.GetByIdError(),
            new ProveedorRepositoryErrors.ExistsByNameError()
        ];

        // Act & Assert
        errores.Should().HaveCount(6);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void CatalogoServiceErrors_Constructores_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            new CatalogoServiceErrors.CategoriaConFunkosError("abcdefghijk"),
            new CatalogoServiceErrors.ProveedorConFunkosError("64b000000000000000000001"),
            new CatalogoServiceErrors.ProveedorIdInvalidoError("no-es-un-objectid")
        ];

        // Act & Assert
        errores.Should().HaveCount(3);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void CatalogoServiceError_Fabricas_CreanTodosLosErrores() {
        // Arrange
        DomainErrors[] errores = [
            CatalogoServiceError.CategoriaConFunkosError("abcdefghijk"),
            CatalogoServiceError.ProveedorConFunkosError("64b000000000000000000001"),
            CatalogoServiceError.ProveedorIdInvalidoError("no-es-un-objectid")
        ];

        // Act & Assert
        errores.Should().HaveCount(3);
        errores.Should().AllSatisfy(error => error.Message.Should().NotBeNullOrWhiteSpace());
    }

    [Test]
    public void AuthErrors_Constructor_CreaElErrorDeCredenciales() {
        // Arrange & Act
        var error = new AuthErrors.InvalidCredentialsError();

        // Assert
        error.Message.Should().NotBeNullOrWhiteSpace();
    }
}