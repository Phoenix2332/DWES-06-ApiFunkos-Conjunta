using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Categorias;

[TestFixture]
public class CategoriaControllerCasosCorrectosTest : CategoriaControllerTestBase {
    [Test]
    public async Task GetAll_ConParametrosValidos_RetornaOkConCategorias() {
        // Arrange
        var datos = CreatePagedResult();

        Service
            .Setup(s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<(IReadOnlyList<CategoriaDto> Categorias, int Total), DomainErrors>(datos));

        // Act
        var resultado = await Controller.GetAll(null, 1, 10, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().BeOfType<PagedResult<CategoriaDto>>();

        var pagedResult = (PagedResult<CategoriaDto>)okResult.Value!;

        pagedResult.Datos.Should().HaveCount(1);
        pagedResult.Total.Should().Be(1);
        pagedResult.Pagina.Should().Be(1);
        pagedResult.TamanoPagina.Should().Be(10);
    }

    [Test]
    public async Task GetById_CategoriaExistente_RetornaOk() {
        // Arrange
        var categoria = CreateCategoria();

        Service
            .Setup(s => s.GetByIdAsync(categoria.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await Controller.GetById(categoria.Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().Be(categoria);
    }

    [Test]
    public async Task Create_DatosValidos_RetornaCreated() {
        // Arrange
        var request = CreateCreateRequest();
        var categoria = CreateCategoria();

        Service
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = (CreatedAtActionResult)resultado;

        createdResult.ActionName.Should().Be(nameof(Controller.GetById));
        createdResult.RouteValues!["id"].Should().Be(categoria.Id);
        createdResult.Value.Should().Be(categoria);
    }

    [Test]
    public async Task Update_DatosValidos_RetornaOk() {
        // Arrange
        const string Id = "abcdefghijk";
        var request = CreateUpdateRequest();
        var categoria = CreateCategoria();

        Service
            .Setup(s => s.UpdateAsync(Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<CategoriaDto, DomainErrors>(categoria));

        // Act
        var resultado = await Controller.Update(Id, request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().Be(categoria);
    }

    [Test]
    public async Task Delete_CategoriaExistente_RetornaNoContent() {
        // Arrange
        const string Id = "abcdefghijk";

        Service
            .Setup(s => s.DeleteAsync(Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await Controller.Delete(Id, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();

        var noContentResult = (NoContentResult)resultado;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}