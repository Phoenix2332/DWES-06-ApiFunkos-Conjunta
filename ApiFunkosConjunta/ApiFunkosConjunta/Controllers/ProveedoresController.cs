using ApiFunkosConjunta.Common.Examples;
using ApiFunkosConjunta.Common.Http;
using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Proveedores;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Controllers;

/// <summary>
/// Endpoints REST de los proveedores del catalogo.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Gestion de los proveedores del catalogo: listado paginado, consulta, alta, modificacion y baja.")]
public class ProveedoresController(ICatalogoService service) : ControllerBase {
    /// <summary>
    /// Lista los proveedores del catalogo con paginacion y filtro opcional por nombre.
    /// </summary>
    /// <param name="nombre">Filtro por nombre del proveedor (no distingue mayusculas). Opcional.</param>
    /// <param name="pagina">Numero de pagina. Empieza en 1.</param>
    /// <param name="tamanoPagina">Numero de proveedores por pagina. Entre 1 y 100.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>Lista paginada de proveedores.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar proveedores",
        Description = "Devuelve los proveedores del catalogo paginados. Admite un filtro por nombre.",
        OperationId = "GetAllProveedores"
    )]
    [SwaggerResponse(200, "Lista paginada de proveedores", typeof(PagedResult<ProveedorDto>))]
    [SwaggerResponse(400, "Los parametros de paginacion no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> GetAll(
        [FromQuery, SwaggerParameter("Filtro por nombre del proveedor. Opcional.", Required = false)] string? nombre,
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

        var result = await service.GetAllProveedoresAsync(nombre, pagina, tamanoPagina, cancellationToken);

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

    /// <summary>
    /// Obtiene un proveedor por su identificador.
    /// </summary>
    /// <param name="id">Identificador del proveedor (ObjectId de MongoDB, 24 caracteres hexadecimales).</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>El proveedor solicitado.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtener un proveedor por id",
        Description = "Devuelve el proveedor con el identificador indicado.",
        OperationId = "GetProveedorById"
    )]
    [SwaggerResponse(200, "Proveedor encontrado", typeof(ProveedorDto))]
    [SwaggerResponse(400, "El identificador del proveedor no tiene un formato valido", typeof(ProblemDetails))]
    [SwaggerResponse(404, "No existe un proveedor con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> GetById(
        [FromRoute, SwaggerParameter("Identificador del proveedor (ObjectId de MongoDB).", Required = true)] string id,
        CancellationToken cancellationToken) {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(ApiProblem.BadRequest("El id del proveedor no es válido.",
                HttpContext?.Request.Path.Value));

        var result = await service.GetProveedorByIdAsync(objectId, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea un proveedor nuevo. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="dto">Datos del proveedor a crear.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>El proveedor creado, con la cabecera Location apuntando a su recurso.</returns>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Crear un proveedor",
        Description = "Crea un proveedor nuevo en el catalogo.",
        OperationId = "CreateProveedor"
    )]
    [SwaggerRequestExample(typeof(CrearProveedorDto), typeof(CrearProveedorDtoExample))]
    [SwaggerResponse(201, "Proveedor creado correctamente", typeof(ProveedorDto))]
    [SwaggerResponse(400, "Los datos del proveedor no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Ya existe un proveedor con ese nombre", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Create(
        [FromBody, SwaggerRequestBody("Datos del proveedor a crear.", Required = true)] CrearProveedorDto dto,
        CancellationToken cancellationToken) {
        var result = await service.CreateProveedorAsync(dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Actualiza un proveedor existente. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="id">Identificador del proveedor a actualizar (ObjectId de MongoDB).</param>
    /// <param name="dto">Nuevos datos del proveedor.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>El proveedor actualizado.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Actualizar un proveedor",
        Description = "Actualiza todos los campos del proveedor indicado.",
        OperationId = "UpdateProveedor"
    )]
    [SwaggerResponse(200, "Proveedor actualizado correctamente", typeof(ProveedorDto))]
    [SwaggerResponse(400, "El identificador del proveedor no tiene un formato valido", typeof(ProblemDetails))]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(404, "No existe un proveedor con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Ya existe un proveedor con ese nombre", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Update(
        [FromRoute, SwaggerParameter("Identificador del proveedor a actualizar (ObjectId de MongoDB).",
            Required = true)] string id,
        [FromBody, SwaggerRequestBody("Nuevos datos del proveedor.", Required = true)] ActualizarProveedorDto dto,
        CancellationToken cancellationToken) {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(ApiProblem.BadRequest("El id del proveedor no es válido.",
                HttpContext?.Request.Path.Value));

        var result = await service.UpdateProveedorAsync(objectId, dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Elimina un proveedor del catalogo. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="id">Identificador del proveedor a eliminar (ObjectId de MongoDB).</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>Sin contenido si el proveedor se ha eliminado.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Eliminar un proveedor",
        Description = "Elimina el proveedor indicado del catalogo. No se permite si tiene Funkos asociados.",
        OperationId = "DeleteProveedor"
    )]
    [SwaggerResponse(204, "Proveedor eliminado correctamente")]
    [SwaggerResponse(400, "El identificador del proveedor no tiene un formato valido", typeof(ProblemDetails))]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(404, "No existe un proveedor con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(409, "El proveedor tiene Funkos asociados", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(
        [FromRoute, SwaggerParameter("Identificador del proveedor a eliminar (ObjectId de MongoDB).",
            Required = true)] string id,
        CancellationToken cancellationToken) {
        if (!ObjectId.TryParse(id, out var objectId))
            return BadRequest(ApiProblem.BadRequest("El id del proveedor no es válido.",
                HttpContext?.Request.Path.Value));

        var result = await service.DeleteProveedorAsync(objectId, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return result.Value
            ? NoContent()
            : NotFound(ApiProblem.NotFound("No se ha encontrado el proveedor.", HttpContext?.Request.Path.Value));
    }

    private IActionResult HandleError(DomainErrors error) {
        return error switch {
            ProveedorServiceErrors.NotFoundError => NotFound(
                ApiProblem.NotFound(error.Message, HttpContext?.Request.Path.Value)),
            ProveedorServiceErrors.DuplicateNameError => Conflict(
                ApiProblem.Conflict(error.Message, HttpContext?.Request.Path.Value)),
            CatalogoServiceErrors.ProveedorConFunkosError => Conflict(
                ApiProblem.Conflict(error.Message, HttpContext?.Request.Path.Value)),
            _ => Problem(
                title: "Se ha producido un error interno.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext?.Request.Path.Value,
                type: ApiProblem.TypeInternalServerError)
        };
    }
}
