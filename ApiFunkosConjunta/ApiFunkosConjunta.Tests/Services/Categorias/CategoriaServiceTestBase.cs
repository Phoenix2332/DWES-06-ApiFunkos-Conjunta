using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Implementations;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Categorias;

public abstract class CategoriaServiceTestBase {
    protected Mock<ICategoriaRepository> Repository = null!;
    protected CategoriaService Service = null!;

    [SetUp]
    public void SetUp() {
        Repository = new Mock<ICategoriaRepository>();
        Service = new CategoriaService(Repository.Object);
    }

    protected static Categoria CreateCategoria(string id = "abcdefghijk") {
        return new Categoria {
            Id = id,
            Nombre = "Videojuegos"
        };
    }

    protected static CrearCategoriaDto CreateRequest() {
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