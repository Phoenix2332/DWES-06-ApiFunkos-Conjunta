using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiFunkosConjunta.Dto.Auth;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;

namespace ApiFunkosConjunta.Tests.Services.Auth;

[TestFixture]
public class AuthServiceCasosCorrectosTest : AuthServiceTestBase {
    [Test]
    public async Task Login_CredencialesCorrectas_RetornaTokenValido() {
        // Arrange
        const string Username = "administrador";
        const string Password = "password-test";
        const string Rol = "Administrador";

        var usuario = await CreateUsuarioAsync(Username, Password, Rol);

        var dto = new LoginDto {
            Username = Username,
            Password = Password
        };

        // Act
        var resultado = await Service.Login(dto);

        // Assert
        resultado.IsSuccess.Should().BeTrue();
        resultado.Value.Should().NotBeNull();
        resultado.Value.Token.Should().NotBeNullOrWhiteSpace();

        var tokenHandler = new JwtSecurityTokenHandler();

        var validationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = JwtIssuer,
            ValidateAudience = true,
            ValidAudience = JwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtKey)),
            ValidateLifetime = true
        };

        var principal = tokenHandler.ValidateToken(
            resultado.Value.Token,
            validationParameters,
            out var validatedToken);

        validatedToken.Should().BeOfType<JwtSecurityToken>();

        principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            .Should().Be(usuario.Id.ToString());

        principal.FindFirst(ClaimTypes.Name)?.Value
            .Should().Be(Username);

        principal.FindFirst(ClaimTypes.Role)?.Value
            .Should().Be(Rol);
    }
}