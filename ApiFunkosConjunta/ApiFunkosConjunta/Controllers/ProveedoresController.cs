using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace ApiFunkosConjunta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProveedoresController(IProveedorService service) : ControllerBase {
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? nombre, [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10, CancellationToken cancellationToken = default) {
        if (pagina < 1 || tamanoPagina < 1 || tamanoPagina > 100)
            return BadRequest("Los parámetros de paginación no son válidos.");

        var result = await service.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        var resultado = new PagedResult<ProveedorDto> {
            Datos = result.Value.Proveedores,
            Total = result.Value.Total,
            Pagina = pagina,
            TamanoPagina = tamanoPagina
        };

        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken) {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("El id del proveedor no es válido.");

        var result = await service.GetByIdAsync(objectId, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearProveedorDto dto, CancellationToken cancellationToken) {
        var result = await service.CreateAsync(dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ActualizarProveedorDto dto,
        CancellationToken cancellationToken) {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("El id del proveedor no es válido.");

        var result = await service.UpdateAsync(objectId, dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken) {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest("El id del proveedor no es válido.");

        var result = await service.DeleteAsync(objectId, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return result.Value
            ? NoContent()
            : NotFound("No se ha encontrado el proveedor.");
    }

    private IActionResult HandleError(DomainErrors error) {
        return error switch {
            ProveedorServiceErrors.NotFoundError => NotFound(error.Message),
            ProveedorServiceErrors.DuplicateNameError => Conflict(error.Message),
            _ => Problem(title: "Se ha producido un error interno.",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}
