using System.Security.Claims;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Funkos;

[TestFixture]
public class FunkoControllerNotificacionesTest : FunkoControllerTestBase {
    [Test]
    public async Task Create_SinClaimDeUsuario_RetornaUnauthorized() {
        // Arrange
        var request = CreateCreateRequest();

        Controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<UnauthorizedObjectResult>();

        var problemDetails = ((UnauthorizedObjectResult)resultado).Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Status.Should().Be(StatusCodes.Status401Unauthorized);

        Service.Verify(
            s => s.CreateFunkoAsync(It.IsAny<CrearFunkoDto>(), It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Test]
    public async Task Create_CategoriaInexistente_RetornaNotFound() {
        // Arrange
        var request = CreateCreateRequest();

        Controller.ControllerContext = new ControllerContext {
            HttpContext = new DefaultHttpContext {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.NameIdentifier, UsuarioId.ToString())
                        ],
                        "TestAuthentication"))
            }
        };

        Service
            .Setup(s => s.CreateFunkoAsync(request, UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<FunkoDto, DomainErrors>(ServiceError.NotFoundError("abcdefghijk")));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NotFoundObjectResult>();

        var problemDetails = ((NotFoundObjectResult)resultado).Value.Should().BeOfType<ProblemDetails>().Subject;

        problemDetails.Status.Should().Be(StatusCodes.Status404NotFound);
    }
}
