using ApiFunkosConjunta.Common.Examples;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Common.Examples;

[TestFixture]
public class ExamplesTest {
    [Test]
    public void CrearFunkoDtoExample_GetExamples_DevuelveElFunkoDeEjemplo() {
        // Arrange & Act
        var ejemplo = new CrearFunkoDtoExample().GetExamples();

        // Assert
        ejemplo.Nombre.Should().Be("Funko Pop! Batman");
        ejemplo.CategoriaId.Should().Be("D2M_Lk6kKe6");
        ejemplo.ProveedorId.Should().Be("64b000000000000000000002");
        ejemplo.Stock.Should().Be(25);
        ejemplo.Precio.Should().Be(24.99m);
        ejemplo.Imagen.Should().Be("imagenes/batman.png");
    }

    [Test]
    public void ActualizarFunkoDtoExample_GetExamples_DevuelveElFunkoDeEjemplo() {
        // Arrange & Act
        var ejemplo = new ActualizarFunkoDtoExample().GetExamples();

        // Assert
        ejemplo.Nombre.Should().Be("Funko Pop! Batman (edicion 2026)");
        ejemplo.Stock.Should().Be(12);
        ejemplo.Precio.Should().Be(29.99m);
    }

    [Test]
    public void CrearCategoriaDtoExample_GetExamples_DevuelveLaCategoriaDeEjemplo() {
        // Arrange & Act
        var ejemplo = new CrearCategoriaDtoExample().GetExamples();

        // Assert
        ejemplo.Nombre.Should().Be("Videojuegos");
    }

    [Test]
    public void CrearProveedorDtoExample_GetExamples_DevuelveElProveedorDeEjemplo() {
        // Arrange & Act
        var ejemplo = new CrearProveedorDtoExample().GetExamples();

        // Assert
        ejemplo.Nombre.Should().Be("Funko");
        ejemplo.Contacto.Should().Be("ventas@funko.com");
        ejemplo.Pais.Should().Be("Estados Unidos");
    }

    [Test]
    public void LoginDtoExample_GetExamples_DevuelveLasCredencialesDeEjemplo() {
        // Arrange & Act
        var ejemplo = new LoginDtoExample().GetExamples();

        // Assert
        ejemplo.Username.Should().Be("admin");
        ejemplo.Password.Should().Be("Admin123!");
    }
}
