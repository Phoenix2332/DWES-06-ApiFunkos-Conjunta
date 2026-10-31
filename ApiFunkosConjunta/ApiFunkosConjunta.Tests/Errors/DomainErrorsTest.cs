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
}