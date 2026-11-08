using ApiFunkosConjunta.Controllers;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Services;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Funkos;

public abstract class FunkoControllerTestBase {
    protected const int UsuarioId = 1;
    protected FunkosController Controller = null!;
    protected Mock<ICatalogoService> Service = null!;

    [SetUp]
    public void SetUp() {
        Service = new Mock<ICatalogoService>();
        Controller = new FunkosController(Service.Object);
    }

    protected static FunkoDto CreateFunko(int id = 1) {
        return new FunkoDto {
            Id = id,
            Nombre = "Funko de prueba",
            CategoriaId = "abcdefghijk",
            ProveedorId = "64b000000000000000000001",
            Stock = 10,
            Precio = 19.99m,
            Imagen = "imagenes/funko-prueba.png"
        };
    }

    protected static (IReadOnlyList<FunkoDto> Funkos, int Total) CreatePagedResult() {
        IReadOnlyList<FunkoDto> funkos = [
            CreateFunko()
        ];

        return (funkos, 1);
    }

    protected static CrearFunkoDto CreateCreateRequest() {
        return new CrearFunkoDto {
            Nombre = "Funko de prueba",
            CategoriaId = "abcdefghijk",
            ProveedorId = "64b000000000000000000001",
            Stock = 10,
            Precio = 19.99m,
            Imagen = "imagenes/funko-prueba.png"
        };
    }

    protected static ActualizarFunkoDto CreateUpdateRequest() {
        return new ActualizarFunkoDto {
            Nombre = "Funko actualizado",
            CategoriaId = "abcdefghijk",
            ProveedorId = "64b000000000000000000001",
            Stock = 20,
            Precio = 24.99m,
            Imagen = "imagenes/funko-actualizado.png"
        };
    }
}