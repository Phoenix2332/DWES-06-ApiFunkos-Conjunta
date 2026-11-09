using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Common.Pagination;

/// <summary>
/// Resultado paginado de un listado de la API.
/// </summary>
/// <typeparam name="T">Tipo de los elementos devueltos.</typeparam>
[SwaggerSchema(Description = "Resultado paginado de un listado de la API.")]
public class PagedResult<T> {
    /// <summary>
    /// Elementos de la pagina actual.
    /// </summary>
    [SwaggerSchema(Description = "Elementos de la página actual.")]
    public IReadOnlyList<T> Datos { get; set; } = [];

    /// <summary>
    /// Numero total de elementos que cumplen el filtro, sin paginar.
    /// </summary>
    /// <example>42</example>
    [SwaggerSchema(Description = "Número total de elementos que cumplen el filtro.", Format = "int32")]
    public int Total { get; set; }

    /// <summary>
    /// Numero de pagina devuelta.
    /// </summary>
    /// <example>1</example>
    [SwaggerSchema(Description = "Número de página devuelta.", Format = "int32")]
    public int Pagina { get; set; }

    /// <summary>
    /// Numero de elementos por pagina.
    /// </summary>
    /// <example>10</example>
    [SwaggerSchema(Description = "Número de elementos por página.", Format = "int32")]
    public int TamanoPagina { get; set; }

    /// <summary>
    /// Numero total de paginas del listado.
    /// </summary>
    /// <example>5</example>
    [SwaggerSchema(Description = "Número total de páginas del listado.", Format = "int32")]
    public int TotalPaginas => (int)Math.Ceiling((double)Total / TamanoPagina);
}
