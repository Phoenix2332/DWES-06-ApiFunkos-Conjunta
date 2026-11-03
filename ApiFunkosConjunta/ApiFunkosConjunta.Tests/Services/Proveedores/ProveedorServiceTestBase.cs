using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Implementations;
using Moq;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Tests.Services.Proveedores;

public abstract class ProveedorServiceTestBase {
    protected Mock<IProveedorRepository> Repository = null!;
    protected ProveedorService Service = null!;

    [SetUp]
    public void SetUp() {
        Repository = new Mock<IProveedorRepository>();
        Service = new ProveedorService(Repository.Object);
    }

    protected static Proveedor CreateProveedor(string id = "64b0000000000000000000ff", string nombre = "Logitech") {
        return new Proveedor {
            Id = ObjectId.Parse(id),
            Nombre = nombre,
            Contacto = "exportaciones@logitech.com",
            Pais = "Suiza"
        };
    }

    protected static CrearProveedorDto CreateRequest() {
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
