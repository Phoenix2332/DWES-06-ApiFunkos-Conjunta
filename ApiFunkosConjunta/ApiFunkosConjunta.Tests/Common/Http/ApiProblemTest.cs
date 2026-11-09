using ApiFunkosConjunta.Common.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace ApiFunkosConjunta.Tests.Common.Http;

[TestFixture]
public class ApiProblemTest {
    [Test]
    public void NotFound_ConstruyeElProblemDetailsConElStatusYTipoDelRfc9457() {
        // Arrange
        const string Detalle = "No se ha encontrado el Funko.";
        const string Instancia = "/api/funkos/1";

        // Act
        var problem = ApiProblem.NotFound(Detalle, Instancia);

        // Assert
        problem.Status.Should().Be(StatusCodes.Status404NotFound);
        problem.Type.Should().Be("https://tools.ietf.org/html/rfc9457#section-10.5");
        problem.Title.Should().Be("No se ha encontrado el recurso solicitado.");
        problem.Detail.Should().Be(Detalle);
        problem.Instance.Should().Be(Instancia);
    }

    [Test]
    public void Conflict_ConstruyeElProblemDetailsConElStatusYTipoDelRfc9457() {
        // Arrange
        const string Detalle = "Ya existe una categoria con el nombre: Videojuegos.";

        // Act
        var problem = ApiProblem.Conflict(Detalle);

        // Assert
        problem.Status.Should().Be(StatusCodes.Status409Conflict);
        problem.Type.Should().Be("https://tools.ietf.org/html/rfc9457#section-10.8");
        problem.Detail.Should().Be(Detalle);
    }

    [Test]
    public void Unauthorized_ConstruyeElProblemDetailsConElStatusYTipoDelRfc9457() {
        // Act
        var problem = ApiProblem.Unauthorized("El usuario o la contrasena son incorrectos.");

        // Assert
        problem.Status.Should().Be(StatusCodes.Status401Unauthorized);
        problem.Type.Should().Be("https://tools.ietf.org/html/rfc9457#section-10.3");
    }

    [Test]
    public void InternalServerError_ConstruyeElProblemDetailsConElStatusYTipoDelRfc9457() {
        // Act
        var problem = ApiProblem.InternalServerError();

        // Assert
        problem.Status.Should().Be(StatusCodes.Status500InternalServerError);
        problem.Type.Should().Be(ApiProblem.TypeInternalServerError);
        problem.Title.Should().Be("Se ha producido un error interno.");
    }

    [Test]
    public void Validacion_ConstruyeElValidationProblemDetailsConLosErroresDeCampo() {
        // Arrange
        var errores = new Dictionary<string, string[]> {
            ["paginacion"] = ["La pagina debe ser mayor o igual que 1."]
        };

        // Act
        var problem = ApiProblem.Validacion("Parametros de paginacion no validos.", errores);

        // Assert
        problem.Status.Should().Be(StatusCodes.Status400BadRequest);
        problem.Title.Should().Be("Parametros de paginacion no validos.");
        problem.Errors.Should().ContainKey("paginacion");
        problem.Errors["paginacion"].Should().ContainSingle();
    }
}
