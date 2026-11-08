using System.Security.Claims;
using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Funkos;

[TestFixture]
public class FunkoControllerCasosCorrectosTest : FunkoControllerTestBase {
    [Test]
    public async Task GetAll_ConParametrosValidos_RetornaOkConFunkos() {
        // Arrange
        var datos = CreatePagedResult();

        Service
            .Setup(s => s.GetAllFunkosAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>(datos));

        // Act
        var resultado = await Controller.GetAll(null, 1, 10, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().BeOfType<PagedResult<FunkoDto>>();

        var pagedResult = (PagedResult<FunkoDto>)okResult.Value;

        pagedResult.Datos.Should().HaveCount(1);
        pagedResult.Total.Should().Be(1);
        pagedResult.Pagina.Should().Be(1);
        pagedResult.TamanoPagina.Should().Be(10);
    }

    [Test]
    public async Task GetById_FunkoExistente_RetornaOk() {
        // Arrange
        var funko = CreateFunko();

        Service
            .Setup(s => s.GetFunkoByIdAsync(funko.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await Controller.GetById(funko.Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().Be(funko);
    }

    [Test]
    public async Task Create_DatosValidos_RetornaCreated() {
        // Arrange
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

        var request = CreateCreateRequest();
        var funko = CreateFunko();

        Service
            .Setup(s => s.CreateFunkoAsync(request, UsuarioId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = (CreatedAtActionResult)resultado;

        createdResult.ActionName.Should().Be(nameof(Controller.GetById));
        createdResult.RouteValues!["id"].Should().Be(funko.Id);
        createdResult.Value.Should().Be(funko);
    }

    [Test]
    public async Task Update_DatosValidos_RetornaOk() {
        // Arrange
        const int Id = 1;
        var request = CreateUpdateRequest();
        var funko = CreateFunko();

        Service
            .Setup(s => s.UpdateFunkoAsync(Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<FunkoDto, DomainErrors>(funko));

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().Be(funko);
    }

    [Test]
    public async Task Delete_FunkoExistente_RetornaNoContent() {
        // Arrange
        const int Id = 1;

        Service
            .Setup(s => s.DeleteFunkoAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await Controller.Delete(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();

        var noContentResult = (NoContentResult)resultado;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}