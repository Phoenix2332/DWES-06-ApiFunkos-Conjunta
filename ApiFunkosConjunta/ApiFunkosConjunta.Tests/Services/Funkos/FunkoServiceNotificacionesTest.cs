using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Services.Email;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiFunkosConjunta.Tests.Services.Funkos;

/// <summary>
/// Pruebas de las notificaciones (email y SignalR) que dispara el servicio al crear,
/// actualizar y eliminar un Funko.
/// </summary>
[TestFixture]
public class FunkoServiceNotificacionesTest : FunkoServiceTestBase {
    [Test]
    public async Task CreateAsync_DatosValidos_NotificaPorSignalRALosAdministradores() {
        // Arrange
        var request = CreateRequest();
        var funko = CreateFunko();

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko, DomainErrors>(funko));

        // Act
        var result = await Service.CreateAsync(request, UsuarioId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        HubClients.Verify(c => c.Group("administradores"), Times.Once);

        ClientProxy.Verify(
            c => c.SendCoreAsync("FunkoCreado", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task UpdateAsync_FunkoExistente_NotificaPorSignalRALosAdministradoresYAlUsuario() {
        // Arrange
        var funko = CreateFunko();
        funko.UsuarioId = UsuarioId;

        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.UpdateAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funko f, CancellationToken _) => Result.Success<Funko, DomainErrors>(f));

        // Act
        var result = await Service.UpdateAsync(funko.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        HubClients.Verify(c => c.Group("administradores"), Times.Once);
        HubClients.Verify(c => c.Group($"usuario-{UsuarioId}"), Times.Once);

        ClientProxy.Verify(
            c => c.SendCoreAsync("FunkoActualizado", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Test]
    public async Task DeleteAsync_FunkoExistente_NotificaPorSignalRALosAdministradoresYAlUsuario() {
        // Arrange
        var funko = CreateFunko();
        funko.UsuarioId = UsuarioId;

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.DeleteAsync(funko, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var result = await Service.DeleteAsync(funko.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        HubClients.Verify(c => c.Group("administradores"), Times.Once);
        HubClients.Verify(c => c.Group($"usuario-{UsuarioId}"), Times.Once);

        ClientProxy.Verify(
            c => c.SendCoreAsync("FunkoEliminado", It.IsAny<object?[]>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Test]
    public async Task CreateAsync_SiElEmailFalla_CreaElFunkoYRegistraElAviso() {
        // Arrange
        var request = CreateRequest();
        var funko = CreateFunko();

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko, DomainErrors>(funko));

        EmailService
            .Setup(e => e.EnqueueEmailAsync(It.IsAny<EmailMessage>()))
            .ThrowsAsync(new InvalidOperationException("El servidor SMTP no esta disponible."));

        // Act
        var result = await Service.CreateAsync(request, UsuarioId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(funko.Id);

        Logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task CreateAsync_SiSignalRFalla_CreaElFunkoYRegistraElAviso() {
        // Arrange
        var request = CreateRequest();
        var funko = CreateFunko();

        Repository
            .Setup(r => r.AddAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko, DomainErrors>(funko));

        ClientProxy
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("El hub de SignalR no esta disponible."));

        // Act
        var result = await Service.CreateAsync(request, UsuarioId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task UpdateAsync_SiSignalRFalla_ActualizaElFunkoYRegistraElAviso() {
        // Arrange
        var funko = CreateFunko();
        var request = CreateUpdateRequest();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.UpdateAsync(It.IsAny<Funko>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Funko f, CancellationToken _) => Result.Success<Funko, DomainErrors>(f));

        ClientProxy
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("El hub de SignalR no esta disponible."));

        // Act
        var result = await Service.UpdateAsync(funko.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_SiSignalRFalla_EliminaElFunkoYRegistraElAviso() {
        // Arrange
        var funko = CreateFunko();

        Repository
            .Setup(r => r.GetByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<Funko?, DomainErrors>(funko));

        Repository
            .Setup(r => r.DeleteAsync(funko, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        ClientProxy
            .Setup(c => c.SendCoreAsync(
                It.IsAny<string>(),
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("El hub de SignalR no esta disponible."));

        // Act
        var result = await Service.DeleteAsync(funko.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();

        Logger.Verify(
            l => l.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
