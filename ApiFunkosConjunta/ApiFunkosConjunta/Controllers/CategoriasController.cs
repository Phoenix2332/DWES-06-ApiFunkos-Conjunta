using ApiFunkosConjunta.Common.Examples;
using ApiFunkosConjunta.Common.Http;
using ApiFunkosConjunta.Common.Pagination;
using ApiFunkosConjunta.Dto.Categorias;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Controllers;

/// <summary>
/// Endpoints REST de las categorias del catalogo.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Gestion de las categorias del catalogo: listado paginado, consulta, alta, modificacion y baja.")]
public class CategoriasController(ICatalogoService service) : ControllerBase {
    /// <summary>
    /// Lista las categorias del catalogo con paginacion y filtro opcional por nombre.
    /// </summary>
    /// <param name="nombre">Filtro por nombre de la categoria (no distingue mayusculas). Opcional.</param>
    /// <param name="pagina">Numero de pagina. Empieza en 1.</param>
    /// <param name="tamanoPagina">Numero de categorias por pagina. Entre 1 y 100.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>Lista paginada de categorias.</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Listar categorias",
        Description = "Devuelve las categorias del catalogo paginadas. Admite un filtro por nombre.",
        OperationId = "GetAllCategorias"
    )]
    [SwaggerResponse(200, "Lista paginada de categorias", typeof(PagedResult<CategoriaDto>))]
    [SwaggerResponse(400, "Los parametros de paginacion no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> GetAll(
        [FromQuery, SwaggerParameter("Filtro por nombre de la categoria. Opcional.", Required = false)] string? nombre,
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

        var result = await service.GetAllCategoriasAsync(nombre, pagina, tamanoPagina, cancellationToken);

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

    /// <summary>
    /// Obtiene una categoria por su identificador.
    /// </summary>
    /// <param name="id">Identificador de la categoria.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>La categoria solicitada.</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Obtener una categoria por id",
        Description = "Devuelve la categoria con el identificador indicado.",
        OperationId = "GetCategoriaById"
    )]
    [SwaggerResponse(200, "Categoria encontrada", typeof(CategoriaDto))]
    [SwaggerResponse(404, "No existe una categoria con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> GetById(
        [FromRoute, SwaggerParameter("Identificador de la categoria.", Required = true)] string id,
        CancellationToken cancellationToken) {
        var result = await service.GetCategoriaByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Crea una categoria nueva. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="dto">Datos de la categoria a crear.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>La categoria creada, con la cabecera Location apuntando a su recurso.</returns>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Crear una categoria",
        Description = "Crea una categoria nueva en el catalogo.",
        OperationId = "CreateCategoria"
    )]
    [SwaggerRequestExample(typeof(CrearCategoriaDto), typeof(CrearCategoriaDtoExample))]
    [SwaggerResponse(201, "Categoria creada correctamente", typeof(CategoriaDto))]
    [SwaggerResponse(400, "Los datos de la categoria no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Ya existe una categoria con ese nombre", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Create(
        [FromBody, SwaggerRequestBody("Datos de la categoria a crear.", Required = true)] CrearCategoriaDto dto,
        CancellationToken cancellationToken) {
        var result = await service.CreateCategoriaAsync(dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
    }

    /// <summary>
    /// Actualiza una categoria existente. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="id">Identificador de la categoria a actualizar.</param>
    /// <param name="dto">Nuevos datos de la categoria.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>La categoria actualizada.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Actualizar una categoria",
        Description = "Actualiza todos los campos de la categoria indicada.",
        OperationId = "UpdateCategoria"
    )]
    [SwaggerResponse(200, "Categoria actualizada correctamente", typeof(CategoriaDto))]
    [SwaggerResponse(400, "Los datos de la categoria no son validos", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(404, "No existe una categoria con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(409, "Ya existe una categoria con ese nombre", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Update(
        [FromRoute, SwaggerParameter("Identificador de la categoria a actualizar.", Required = true)] string id,
        [FromBody, SwaggerRequestBody("Nuevos datos de la categoria.", Required = true)] ActualizarCategoriaDto dto,
        CancellationToken cancellationToken) {
        var result = await service.UpdateCategoriaAsync(id, dto, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return Ok(result.Value);
    }

    /// <summary>
    /// Elimina una categoria del catalogo. Solo los usuarios con rol Administrador pueden hacerlo.
    /// </summary>
    /// <param name="id">Identificador de la categoria a eliminar.</param>
    /// <param name="cancellationToken">Token de cancelacion de la peticion.</param>
    /// <returns>Sin contenido si la categoria se ha eliminado.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrador")]
    [SwaggerOperation(
        Summary = "Eliminar una categoria",
        Description = "Elimina la categoria indicada del catalogo. No se permite si tiene Funkos asociados.",
        OperationId = "DeleteCategoria"
    )]
    [SwaggerResponse(204, "Categoria eliminada correctamente")]
    [SwaggerResponse(401, "El usuario no esta autenticado", typeof(ProblemDetails))]
    [SwaggerResponse(403, "El usuario no tiene el rol Administrador", typeof(ProblemDetails))]
    [SwaggerResponse(404, "No existe una categoria con ese identificador", typeof(ProblemDetails))]
    [SwaggerResponse(409, "La categoria tiene Funkos asociados", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(
        [FromRoute, SwaggerParameter("Identificador de la categoria a eliminar.", Required = true)] string id,
        CancellationToken cancellationToken) {
        var result = await service.DeleteCategoriaAsync(id, cancellationToken);

        if (result.IsFailure)
            return HandleError(result.Error);

        return result.Value
            ? NoContent()
            : NotFound(ApiProblem.NotFound("No se ha encontrado la categoria.", HttpContext?.Request.Path.Value));
    }

    private IActionResult HandleError(DomainErrors error) {
        return error switch {
            ServiceErrors.NotFoundError => NotFound(ApiProblem.NotFound(error.Message, HttpContext?.Request.Path.Value)),
            ServiceErrors.DuplicateNameError => Conflict(ApiProblem.Conflict(error.Message, HttpContext?.Request.Path.Value)),
            CatalogoServiceErrors.CategoriaConFunkosError => Conflict(
                ApiProblem.Conflict(error.Message, HttpContext?.Request.Path.Value)),
            _ => Problem(
                title: "Se ha producido un error interno.",
                statusCode: StatusCodes.Status500InternalServerError,
                instance: HttpContext?.Request.Path.Value,
                type: ApiProblem.TypeInternalServerError)
        };
    }
}
