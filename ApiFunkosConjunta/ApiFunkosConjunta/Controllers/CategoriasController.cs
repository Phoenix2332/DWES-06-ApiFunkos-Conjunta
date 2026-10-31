using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiFunkosConjunta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController(ICategoriaService service) : ControllerBase {
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? nombre, [FromQuery] int pagina = 1,
        [FromQuery] int tamanoPagina = 10, CancellationToken cancellationToken = default) {
        if (pagina < 1 || tamanoPagina < 1 || tamanoPagina > 100)
            return BadRequest("Los parámetros de paginación no son válidos.");

        var result = await service.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        var resultado = new PagedResult<CategoriaDto> {
            Datos = result.Value.Categorias,
            Total = result.Value.Total,
            Pagina = pagina,
            TamanoPagina = tamanoPagina
        };

        return Ok(resultado);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken) {
        var result = await service.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCategoriaDto dto, CancellationToken cancellationToken) {
        var result = await service.CreateAsync(dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ActualizarCategoriaDto dto,
        CancellationToken cancellationToken) {
        var result = await service.UpdateAsync(id, dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken) {
        var result = await service.DeleteAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return result.Value
            ? NoContent()
            : NotFound("No se ha encontrado la categoría.");
    }

    private IActionResult HandleError(DomainErrors error) {
        return error switch {
            ServiceErrors.NotFoundError => NotFound(error.Message),
            ServiceErrors.DuplicateNameError => Conflict(error.Message),
            _ => Problem(title: "Se ha producido un error interno.",
                statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}