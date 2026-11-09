using ApiFunkosConjunta.Controllers;
using ApiFunkosConjunta.Dto.Auth;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Auth;

[TestFixture]
public class AuthControllerTest {
    [SetUp]
    public void SetUp() {
        _authService = new Mock<IAuthService>();
        _controller = new AuthController(_authService.Object);
    }

    private Mock<IAuthService> _authService = null!;
    private AuthController _controller = null!;

    [Test]
    public async Task Login_CredencialesCorrectas_RetornaOkConToken() {
        // Arrange
        var dto = new LoginDto {
            Username = "administrador",
            Password = "password-test"
        };

        var respuesta = new LoginResponseDto {
            Token = "token-de-prueba"
        };

        _authService
            .Setup(s => s.Login(dto))
            .ReturnsAsync(Result.Success<LoginResponseDto, DomainErrors>(respuesta));

        // Act
        var resultado = await _controller.Login(dto);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().BeSameAs(respuesta);

        _authService.Verify(s => s.Login(dto), Times.Once);
    }

    [Test]
    public async Task Login_CredencialesIncorrectas_RetornaUnauthorized() {
        // Arrange
        var dto = new LoginDto {
            Username = "administrador",
            Password = "incorrecta"
        };

        var error = new AuthErrors.InvalidCredentialsError();

        _authService
            .Setup(s => s.Login(dto))
            .ReturnsAsync(Result.Failure<LoginResponseDto, DomainErrors>(error));

        // Act
        var resultado = await _controller.Login(dto);

        // Assert
        resultado.Should().BeOfType<UnauthorizedObjectResult>();

        var unauthorizedResult = (UnauthorizedObjectResult)resultado;

        // El error se devuelve como ProblemDetails (RFC 9457).
        var problemDetails = unauthorizedResult.Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Status.Should().Be(StatusCodes.Status401Unauthorized);
        problemDetails.Title.Should().Be("No se ha podido autenticar la peticion.");
        problemDetails.Detail.Should().Be(error.Message);

        _authService.Verify(s => s.Login(dto), Times.Once);
    }
}