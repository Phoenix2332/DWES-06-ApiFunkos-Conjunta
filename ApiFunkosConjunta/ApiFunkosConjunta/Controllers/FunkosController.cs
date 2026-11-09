using System.Security.Claims;
using ApiFunkosConjunta.Common.Examples;
using ApiFunkosConjunta.Common.Http;
using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Controllers;

/// <summary>
/// Endpoints REST del catalogo de Funkos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Gestion del catalogo de Funkos: listado paginado, consulta, alta, modificacion y baja.")]
public class FunkosController(ICatalogoService service) : ControllerBase {
    /// <summary>
    /// Lista los Funkos del catalogo con paginacion y filtro opcional por nombre.
    /// </summary>
    /// <param name="nombre">Filtro por nombre del Funko (no distingue mayusculas). Opcional.</param>
    /// <param name="pagina">Numero de pagina. Empieza en 1.</param>
    /// <param name="tamanoPagina">Numero de Funkos por pagina. Entre 1 y 100.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>Lista paginada de Funkos.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar Funkos",
        Description = "Devuelve los Funkos del catalogo paginados. Admite un filtro por nombre.",
        OperationId = "GetAllFunkos"
    )]
    [SwaggerResponse(200, "Lista paginada de Funkos", typeof(PagedResult<FunkoDto>))]
    [SwaggerResponse(400, "Los parametros de paginacion no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> GetAll(
        [FromQuery, SwaggerParameter("Filtro por nombre del Funko. Opcional.", Required = false)] string? nombre,
        [FromQuery, SwaggerParameter("Numero de pagina (por defecto 1).", Required = false)] int pagina = 1,
        [FromQuery, SwaggerParameter("Tamano de pagina (por defecto 10, maximo 100).", Required = false)]
        int tamanoPagina = 10,
        CancellationToken cancellationToken = default) {
        if (pagina < 1 || tamanoPagina < 1 || tamanoPagina > 100)
            return BadRequest(ApiProblem.Validacion(
                "Parametros de paginacion no validos.",
                new Dictionary<string, string[]> {
                    ["paginacion"] = [
                        "La pagina debe ser mayor o igual que 1 y el tamano de pagina debe estar entre 1 y 100."
                    ]
                },
                HttpContext?.Request.Path.Value));

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

    /// <summary>
    /// Obtiene un Funko por su identificador.
    /// </summary>
    /// <param name="id">Identificador del Funko.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>El Funko solicitado.</returns>
    [HttpGet("{id:int}")]
    [SwaggerOperation(
        Summary = "Obtener un Funko por id",
        Description = "Devuelve el Funko con el identificador indicado.",
        OperationId = "GetFunkoById"
    )]
    [SwaggerResponse(200, "Funko encontrado", typeof(FunkoDto))]
    [SwaggerResponse(404, "No existe un Funko con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> GetById(
        [FromRoute, SwaggerParameter("Identificador del Funko.", Required = true)] int id,
        CancellationToken cancellationToken) {
        var result = await service.GetFunkoByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un Funko nuevo. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="dto">Datos del Funko a crear.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>El Funko creado, con la cabecera Location apuntando a su recurso.</returns>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Crear un Funko",
        Description = "Crea un Funko nuevo en el catalogo. Al crearlo se envia un aviso por email al administrador.",
        OperationId = "CreateFunko"
    )]
    [SwaggerRequestExample(typeof(CrearFunkoDto), typeof(CrearFunkoDtoExample))]
    [SwaggerResponse(201, "Funko creado correctamente", typeof(FunkoDto))]
    [SwaggerResponse(400, "Los datos del Funko no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(404, "La categoria o el proveedor indicados no existen", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Create(
        [FromBody, SwaggerRequestBody("Datos del Funko a crear.", Required = true)] CrearFunkoDto dto,
        CancellationToken cancellationToken) {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized(ApiProblem.Unauthorized(
                "No se ha podido identificar al usuario autenticado.",
                HttpContext?.Request.Path.Value));

        var result = await service.CreateFunkoAsync(dto, usuarioId, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Actualiza un Funko existente. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="id">Identificador del Funko a actualizar.</param>
    /// <param name="dto">Nuevos datos del Funko.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>El Funko actualizado.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Actualizar un Funko",
        Description = "Actualiza todos los campos del Funko indicado.",
        OperationId = "UpdateFunko"
    )]
    [SwaggerRequestExample(typeof(ActualizarFunkoDto), typeof(ActualizarFunkoDtoExample))]
    [SwaggerResponse(200, "Funko actualizado correctamente", typeof(FunkoDto))]
    [SwaggerResponse(400, "Los datos del Funko no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(404, "No existe un Funko con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Update(
        [FromRoute, SwaggerParameter("Identificador del Funko a actualizar.", Required = true)] int id,
        [FromBody, SwaggerRequestBody("Nuevos datos del Funko.", Required = true)] ActualizarFunkoDto dto,
        CancellationToken cancellationToken) {
        var result = await service.UpdateFunkoAsync(id, dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Elimina un Funko del catalogo. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="id">Identificador del Funko a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>Sin contenido si el Funko se ha eliminado.</returns>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Eliminar un Funko",
        Description = "Elimina el Funko indicado del catalogo.",
        OperationId = "DeleteFunko"
    )]
    [SwaggerResponse(204, "Funko eliminado correctamente")]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(404, "No existe un Funko con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(
        [FromRoute, SwaggerParameter("Identificador del Funko a eliminar.", Required = true)] int id,
        CancellationToken cancellationToken) {
        var result = await service.DeleteFunkoAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return result.Value
            ? NoContent()
            : NotFound(ApiProblem.NotFound("No se ha encontrado el Funko.", HttpContext?.Request.Path.Value));
    }

    private IActionResult HandleError(DomainErrors error) {
        return error switch {
            ServiceErrors.NotFoundError => NotFound(ApiProblem.NotFound(error.Message, HttpContext?.Request.Path.Value)),
            _ => Problem(
                title: "Se ha producido un error interno.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext?.Request.Path.Value,
                type: ApiProblem.TypeInternalServerError)
        };
    }
}
