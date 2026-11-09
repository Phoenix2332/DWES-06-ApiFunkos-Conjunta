using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiFunkosConjunta.Common.Http;

/// <summary>
/// Fabricas de <see cref="ProblemDetails"/> (RFC 9457) para las respuestas de error de la API.
/// El campo <c>Type</c> apunta al apartado del RFC que describe el codigo de estado, de modo que
/// cualquier cliente puede interpretar el error de forma generica.
/// </summary>
public static class ApiProblem {
    private const string Base = "https://tools.ietf.org/html/rfc9457#section-";

    /// <summary>
    /// Tipo (URI) del error 500 segun el RFC 9457.
    /// </summary>
    public const string TypeInternalServerError = $"{Base}10.15";

    /// <summary>
    /// 400 Bad Request: los datos o los parametros de la peticion no son validos.
    /// </summary>
    public static ProblemDetails BadRequest(string detail, string? instance = null) {
        return new ProblemDetails {
            Type = $"{Base}10.2",
            Title = "La peticion no es valida.",
            Status = StatusCodes.Status400BadRequest,
            Detail = detail,
            Instance = instance
        };
    }

    /// <summary>
    /// 401 Unauthorized: faltan las credenciales o el token no es valido.
    /// </summary>
    public static ProblemDetails Unauthorized(string detail, string? instance = null) {
        return new ProblemDetails {
            Type = $"{Base}10.3",
            Title = "No se ha podido autenticar la peticion.",
            Status = StatusCodes.Status401Unauthorized,
            Detail = detail,
            Instance = instance
        };
    }

    /// <summary>
    /// 403 Forbidden: el usuario autenticado no tiene permiso para la operacion.
    /// </summary>
    public static ProblemDetails Forbidden(string detail, string? instance = null) {
        return new ProblemDetails {
            Type = $"{Base}10.4",
            Title = "No se tiene permiso para realizar esta operacion.",
            Status = StatusCodes.Status403Forbidden,
            Detail = detail,
            Instance = instance
        };
    }

    /// <summary>
    /// 404 Not Found: el recurso solicitado no existe.
    /// </summary>
    public static ProblemDetails NotFound(string detail, string? instance = null) {
        return new ProblemDetails {
            Type = $"{Base}10.5",
            Title = "No se ha encontrado el recurso solicitado.",
            Status = StatusCodes.Status404NotFound,
            Detail = detail,
            Instance = instance
        };
    }

    /// <summary>
    /// 409 Conflict: el recurso ya existe o tiene operaciones asociadas que lo bloquean.
    /// </summary>
    public static ProblemDetails Conflict(string detail, string? instance = null) {
        return new ProblemDetails {
            Type = $"{Base}10.8",
            Title = "La peticion entra en conflicto con el estado actual del recurso.",
            Status = StatusCodes.Status409Conflict,
            Detail = detail,
            Instance = instance
        };
    }

    /// <summary>
    /// 500 Internal Server Error: error no controlado del servidor.
    /// </summary>
    public static ProblemDetails InternalServerError(string? instance = null) {
        return new ProblemDetails {
            Type = TypeInternalServerError,
            Title = "Se ha producido un error interno.",
            Status = StatusCodes.Status500InternalServerError,
            Instance = instance
        };
    }

    /// <summary>
    /// 400 Bad Request con el diccionario de errores de campo (RFC 9457, tipo ValidationProblemDetails).
    /// </summary>
    public static ValidationProblemDetails Validacion(string title, IDictionary<string, string[]> errores,
        string? instance = null) {
        return new ValidationProblemDetails(errores) {
            Type = $"{Base}10.2",
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Instance = instance
        };
    }
}
