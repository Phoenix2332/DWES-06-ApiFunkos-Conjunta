using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFunkosConjunta.Tests.Controllers.Proveedores;

[TestFixture]
public class ProveedorControllerCasosCorrectosTest : ProveedorControllerTestBase {
    [Test]
    public async Task GetAll_ConParametrosValidos_RetornaOkConProveedores() {
        // Arrange
        var datos = CreatePagedResult();

        Service
            .Setup(s => s.GetAllAsync(null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(
                Result.Success<(IReadOnlyList<ProveedorDto> Proveedores, int Total), DomainErrors>(datos));

        // Act
        var resultado = await Controller.GetAll(null, 1, 10, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().BeOfType<PagedResult<ProveedorDto>>();

        var pagedResult = (PagedResult<ProveedorDto>)okResult.Value!;

        pagedResult.Datos.Should().HaveCount(1);
        pagedResult.Total.Should().Be(1);
        pagedResult.Pagina.Should().Be(1);
        pagedResult.TamanoPagina.Should().Be(10);
    }

    [Test]
    public async Task GetById_ProveedorExistente_RetornaOk() {
        // Arrange
        var proveedor = CreateProveedor();

        Service
            .Setup(s => s.GetByIdAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await Controller.GetById(proveedor.Id.ToString(), CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().Be(proveedor);
    }

    [Test]
    public async Task Create_DatosValidos_RetornaCreated() {
        // Arrange
        var request = CreateCreateRequest();
        var proveedor = CreateProveedor();

        Service
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await Controller.Create(request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<CreatedAtActionResult>();

        var createdResult = (CreatedAtActionResult)resultado;

        createdResult.ActionName.Should().Be(nameof(Controller.GetById));
        createdResult.RouteValues!["id"].Should().Be(proveedor.Id);
        createdResult.Value.Should().Be(proveedor);
    }

    [Test]
    public async Task Update_DatosValidos_RetornaOk() {
        // Arrange
        var proveedor = CreateProveedor();
        var request = CreateUpdateRequest();

        Service
            .Setup(s => s.UpdateAsync(proveedor.Id, request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<ProveedorDto, DomainErrors>(proveedor));

        // Act
        var resultado = await Controller.Update(proveedor.Id.ToString(), request, CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<OkObjectResult>();

        var okResult = (OkObjectResult)resultado;
        okResult.Value.Should().Be(proveedor);
    }

    [Test]
    public async Task Delete_ProveedorExistente_RetornaNoContent() {
        // Arrange
        var proveedor = CreateProveedor();

        Service
            .Setup(s => s.DeleteAsync(proveedor.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success<bool, DomainErrors>(true));

        // Act
        var resultado = await Controller.Delete(proveedor.Id.ToString(), CancellationToken.None);

        // Assert
        resultado.Should().BeOfType<NoContentResult>();

        var noContentResult = (NoContentResult)resultado;
        noContentResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
