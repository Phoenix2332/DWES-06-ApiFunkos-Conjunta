using ApiFunkosConjunta.Common.Configuration;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Hubs;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Email;
using ApiFunkosConjunta.Services.Implementations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Funkos;

public abstract class FunkoServiceTestBase {
    protected const string AdminEmail = "admin@apifunkos.com";
    protected const int UsuarioId = 1;
    protected Mock<IClientProxy> ClientProxy = null!;
    protected Mock<IEmailService> EmailService = null!;
    protected Mock<IHubClients> HubClients = null!;
    protected Mock<IHubContext<FunkoHub>> HubContext = null!;
    protected Mock<ILogger<FunkoService>> Logger = null!;

    protected Mock<IFunkoRepository> Repository = null!;
    protected FunkoService Service = null!;

    [SetUp]
    public void SetUp() {
        Repository = new Mock<IFunkoRepository>();
        EmailService = new Mock<IEmailService>();
        Logger = new Mock<ILogger<FunkoService>>();
        HubContext = new Mock<IHubContext<FunkoHub>>();
        HubClients = new Mock<IHubClients>();
        ClientProxy = new Mock<IClientProxy>();

        HubContext.Setup(h => h.Clients)
            .Returns(HubClients.Object);

        HubClients.Setup(c => c.Group(It.IsAny<string>()))
            .Returns(ClientProxy.Object);

        ClientProxy.Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var smtpSettings = Options.Create(new SmtpSettings { AdminEmail = AdminEmail });

        Service = new FunkoService(Repository.Object, EmailService.Object, smtpSettings, Logger.Object,
            HubContext.Object);
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