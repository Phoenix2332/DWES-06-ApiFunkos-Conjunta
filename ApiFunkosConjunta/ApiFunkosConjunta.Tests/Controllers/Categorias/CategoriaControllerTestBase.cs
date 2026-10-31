using ApiFunkosConjunta.Controllers;
using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Services.Interfaces;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Categorias;

public abstract class CategoriaControllerTestBase {
    protected CategoriasController Controller = null!;
    protected Mock<ICategoriaService> Service = null!;

    [SetUp]
    public void SetUp() {
        Service = new Mock<ICategoriaService>();
        Controller = new CategoriasController(Service.Object);
    }

    protected static CategoriaDto CreateCategoria(string id = "abcdefghijk") {
        return new CategoriaDto {
            Id = id,
            Nombre = "Videojuegos"
        };
    }

    protected static (IReadOnlyList<CategoriaDto> Categorias, int Total) CreatePagedResult() {
        IReadOnlyList<CategoriaDto> categorias = [
            CreateCategoria()
        ];

        return (categorias, 1);
    }

    protected static CrearCategoriaDto CreateCreateRequest() {
        return new CrearCategoriaDto {
            Nombre = "Videojuegos"
        };
    }

    protected static ActualizarCategoriaDto CreateUpdateRequest() {
        return new ActualizarCategoriaDto {
            Nombre = "Figuras"
        };
    }
}