using ApiFunkosConjunta.Dto.Auth;
using ApiFunkosConjunta.Errors;
using FluentAssertions;

namespace ApiFunkosConjunta.Tests.Services.Auth;

[TestFixture]
public class AuthServiceCasosIncorrectosTest : AuthServiceTestBase {
    [Test]
    public async Task Login_UsuarioInexistente_RetornaErrorCredencialesInvalidas() {
        // Arrange
        var dto = new LoginDto {
            Username = "usuario-inexistente",
            Password = "password-test"
        };

        // Act
        var resultado = await Service.Login(dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<AuthErrors.InvalidCredentialsError>();
    }

    [Test]
    public async Task Login_ContrasenaIncorrecta_RetornaErrorCredencialesInvalidas() {
        // Arrange
        await CreateUsuarioAsync();

        var dto = new LoginDto {
            Username = "usuario-test",
            Password = "contrasena-incorrecta"
        };

        // Act
        var resultado = await Service.Login(dto);

        // Assert
        resultado.IsFailure.Should().BeTrue();
        resultado.Error.Should().BeOfType<AuthErrors.InvalidCredentialsError>();
    }
}