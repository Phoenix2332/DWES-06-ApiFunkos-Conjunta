using ApiFunkosConjunta.Common.Pagination;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Common.Pagination;

[TestFixture]
public class PagedResultTest {
    [Test]
    public void TotalPaginas_ConVariosElementos_CalculaLasPaginasRedondeandoHaciaArriba() {
        // Arrange
        var resultado = new PagedResult<string> {
            Datos = ["a", "b"],
            Total = 11,
            Pagina = 1,
            TamanoPagina = 5
        };

        // Act & Assert
        resultado.TotalPaginas.Should().Be(3);
    }

    [Test]
    public void TotalPaginas_SinElementos_DevuelveCero() {
        // Arrange
        var resultado = new PagedResult<string> {
            Total = 0,
            TamanoPagina = 10
        };

        // Act & Assert
        resultado.TotalPaginas.Should().Be(0);
    }

    [Test]
    public void TotalPaginas_ConPaginaExacta_NoRedondea() {
        // Arrange
        var resultado = new PagedResult<string> {
            Total = 20,
            TamanoPagina = 10
        };

        // Act & Assert
        resultado.TotalPaginas.Should().Be(2);
    }
}
