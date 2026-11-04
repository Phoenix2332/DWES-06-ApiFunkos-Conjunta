using ApiFunkosConjunta.Controllers;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Services;
using MongoDB.Bson;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Proveedores;

public abstract class ProveedorControllerTestBase {
    protected ProveedoresController Controller = null!;
    protected Mock<ICatalogoService> Service = null!;

    [SetUp]
    public void SetUp() {
        Service = new Mock<ICatalogoService>();
        Controller = new ProveedoresController(Service.Object);
    }

    protected static ProveedorDto CreateProveedor(string id = "64b0000000000000000000ff") {
        return new ProveedorDto {
            Id = ObjectId.Parse(id),
            Nombre = "Logitech",
            Contacto = "exportaciones@logitech.com",
            Pais = "Suiza"
        };
    }

    protected static (IReadOnlyList<ProveedorDto> Proveedores, int Total) CreatePagedResult() {
        IReadOnlyList<ProveedorDto> proveedores = [
            CreateProveedor()
        ];

        return (proveedores, 1);
    }

    protected static CrearProveedorDto CreateCreateRequest() {
        return new CrearProveedorDto {
            Nombre = "Logitech",
            Contacto = "exportaciones@logitech.com",
            Pais = "Suiza"
        };
    }

    protected static ActualizarProveedorDto CreateUpdateRequest() {
        return new ActualizarProveedorDto {
            Nombre = "Funko",
            Contacto = "ventas@funko.com",
            Pais = "Estados Unidos"
        };
    }
}