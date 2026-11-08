using System.Security.Claims;
using ApiFunkosConjunta.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace ApiFunkosConjunta.Tests.Hubs;

[TestFixture]
public class FunkoHubTest {
    [SetUp]
    public void SetUp() {
        _context = new Mock<HubCallerContext>();
        _groups = new Mock<IGroupManager>();

        _context
            .Setup(c => c.ConnectionId)
            .Returns("conexion-test");

        _hub = new FunkoHub {
            Context = _context.Object,
            Groups = _groups.Object
        };
    }

    [TearDown]
    public void TearDown() {
        _hub.Dispose();
    }

    private Mock<HubCallerContext> _context = null!;
    private Mock<IGroupManager> _groups = null!;
    private FunkoHub _hub = null!;

    [Test]
    public async Task OnConnectedAsync_UsuarioNormal_SeUneASuGrupoPersonal() {
        // Arrange
        var usuario = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "15"),
                    new Claim(ClaimTypes.Role, "Usuario")
                ],
                "Test"
            )
        );

        _context
            .Setup(c => c.User)
            .Returns(usuario);

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _groups.Verify(g => g.AddToGroupAsync("conexion-test", "usuario-15", It.IsAny<CancellationToken>()),
            Times.Once);

        _groups.Verify(g => g.AddToGroupAsync("conexion-test", "administradores", It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task OnConnectedAsync_Administrador_SeUneASusDosGrupos() {
        // Arrange
        var administrador = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ClaimTypes.Role, "Administrador")
                ],
                "Test"
            )
        );

        _context
            .Setup(c => c.User)
            .Returns(administrador);

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _groups.Verify(g => g.AddToGroupAsync("conexion-test", "usuario-1", It.IsAny<CancellationToken>()), Times.Once);

        _groups.Verify(g => g.AddToGroupAsync("conexion-test", "administradores", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task OnConnectedAsync_AdministradorSinId_NoSeUneAGrupoPersonal() {
        // Arrange
        var administrador = new ClaimsPrincipal(
            new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Role, "Administrador")
                ],
                "Test"
            )
        );

        _context
            .Setup(c => c.User)
            .Returns(administrador);

        // Act
        await _hub.OnConnectedAsync();

        // Assert
        _groups.Verify(g => g.AddToGroupAsync("conexion-test", It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _groups.Verify(g => g.AddToGroupAsync("conexion-test", "administradores", It.IsAny<CancellationToken>()),
            Times.Once);
    }
}