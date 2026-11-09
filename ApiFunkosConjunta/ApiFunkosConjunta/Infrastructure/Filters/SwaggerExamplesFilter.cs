using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ApiFunkosConjunta.Infrastructure.Filters;

/// <summary>
/// Filtro de operaciones que completa la documentacion generada por Swashbuckle (apartado 24.9).
/// Anade un ejemplo <c>ProblemDetails</c> a las respuestas de error y un ejemplo generico a los
/// cuerpos de las peticiones que no tengan ya un ejemplo propio.
/// </summary>
public class SwaggerExamplesFilter : IOperationFilter {
    private const string JsonMediaType = "application/json";

    private static readonly string[] _codigosError = ["400", "401", "403", "404", "409", "500"];

    private const string EjemploPeticion = """
        {
          "nombre": "Funko Pop! Batman",
          "categoriaId": "D2M_Lk6kKe6",
          "proveedorId": "64b000000000000000000002",
          "stock": 25,
          "precio": 24.99,
          "imagen": "imagenes/batman.png"
        }
        """;

    /// <summary>
    /// Ejemplos de ProblemDetails (RFC 9457) por codigo de estado.
    /// </summary>
    private static readonly Dictionary<string, string> _ejemplosError = new() {
        ["400"] = """
            {
              "type": "https://tools.ietf.org/html/rfc9457#section-10.2",
              "title": "La peticion no es valida.",
              "status": 400,
              "detail": "El nombre del Funko es obligatorio.",
              "instance": "/api/funkos"
            }
            """,
        ["401"] = """
            {
              "type": "https://tools.ietf.org/html/rfc9457#section-10.3",
              "title": "No se ha podido autenticar la peticion.",
              "status": 401,
              "detail": "No se ha podido identificar al usuario autenticado.",
              "instance": "/api/funkos"
            }
            """,
        ["403"] = """
            {
              "type": "https://tools.ietf.org/html/rfc9457#section-10.4",
              "title": "No se tiene permiso para realizar esta operacion.",
              "status": 403,
              "detail": "El usuario no tiene el rol Administrador.",
              "instance": "/api/funkos"
            }
            """,
        ["404"] = """
            {
              "type": "https://tools.ietf.org/html/rfc9457#section-10.5",
              "title": "No se ha encontrado el recurso solicitado.",
              "status": 404,
              "detail": "No se ha encontrado la categoria con el id: D2M_Lk6kKe6.",
              "instance": "/api/categorias/D2M_Lk6kKe6"
            }
            """,
        ["409"] = """
            {
              "type": "https://tools.ietf.org/html/rfc9457#section-10.8",
              "title": "La peticion entra en conflicto con el estado actual del recurso.",
              "status": 409,
              "detail": "Ya existe una categoria con el nombre: Videojuegos.",
              "instance": "/api/categorias"
            }
            """,
        ["500"] = """
            {
              "type": "https://tools.ietf.org/html/rfc9457#section-10.15",
              "title": "Se ha producido un error interno.",
              "status": 500,
              "detail": "Se ha producido un error interno.",
              "instance": "/api/funkos"
            }
            """
    };

    /// <summary>
    /// Aplica los ejemplos a la operacion documentada.
    /// </summary>
    /// <param name="operation">Operacion de OpenAPI que Swashbuckle esta generando.</param>
    /// <param name="context">Contexto de la operacion (endpoint y metodo).</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        AnadirEjemploPeticion(operation);
        AnadirEjemploErrores(operation);
    }

    private static void AnadirEjemploPeticion(OpenApiOperation operation) {
        if (operation.RequestBody?.Content is null)
            return;

        foreach (var contenido in operation.RequestBody.Content) {
            if (contenido.Key == JsonMediaType && contenido.Value.Example is null)
                contenido.Value.Example = JsonNode.Parse(EjemploPeticion);
        }
    }

    private static void AnadirEjemploErrores(OpenApiOperation operation) {
        if (operation.Responses is not { } respuestas)
            return;

        foreach (var respuesta in respuestas) {
            if (!_codigosError.Contains(respuesta.Key))
                continue;

            var contenido = respuesta.Value?.Content;

            if (contenido is null)
                continue;

            if (!contenido.TryGetValue(JsonMediaType, out var mediaType))
                continue;

            if (mediaType.Example is null && _ejemplosError.TryGetValue(respuesta.Key, out var ejemplo))
                mediaType.Example = JsonNode.Parse(ejemplo);
        }
    }
}
