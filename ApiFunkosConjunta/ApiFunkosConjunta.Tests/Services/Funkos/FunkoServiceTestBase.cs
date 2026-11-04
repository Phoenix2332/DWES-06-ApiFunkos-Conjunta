using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Implementations;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Funkos;

public abstract class FunkoServiceTestBase {
    protected Mock<IFunkoRepository> Repository = null!;
    protected FunkoService Service = null!;

    [SetUp]
    public void SetUp() {
        Repository = new Mock<IFunkoRepository>();
        Service = new FunkoService(Repository.Object);
    }

    protected static Funko CreateFunko(int id = 1) {
        return new Funko {
            Id = id,
            Nombre = "Funko de prueba",
            CategoriaId = "abcdefghijk",
            ProveedorId = "64b000000000000000000001",
            Stock = 10,
            Precio = 19.99m,
            Imagen = "imagenes/funko-prueba.png"
        };
    }

    protected static CrearFunkoDto CreateRequest() {
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
            Stock = 25,
            Precio = 24.99m,
            Imagen = "imagenes/funko-actualizado.png"
        };
    }
}