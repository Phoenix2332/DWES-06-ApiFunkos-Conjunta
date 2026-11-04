using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFunkosConjunta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FunkosController(ICatalogoService service) : ControllerBase {
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? nombre, [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10, CancellationToken cancellationToken = default) {
        if (pagina < 1 || tamanoPagina < 1 || tamanoPagina > 100)
            return BadRequest("Los parámetros de paginación no son válidos.");

        var result = await service.GetAllFunkosAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        var resultado = new PagedResult<FunkoDto> {
            Datos = result.Value.Funkos,
            Total = result.Value.Total,
            Pagina = pagina,
            TamanoPagina = tamanoPagina
        };

        return Ok(resultado);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken) {
        var result = await service.GetFunkoByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Create([FromBody] CrearFunkoDto dto, CancellationToken cancellationToken) {
        var result = await service.CreateFunkoAsync(dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarFunkoDto dto,
        CancellationToken cancellationToken) {
        var result = await service.UpdateFunkoAsync(id, dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) {
        var result = await service.DeleteFunkoAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return result.Value
            ? NoContent()
            : NotFound("No se ha encontrado el Funko.");
    }

    private IActionResult HandleError(DomainErrors error) {
        return error switch {
            ServiceErrors.NotFoundError => NotFound(error.Message),
            _ => Problem(title: "Se ha producido un error interno.",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}