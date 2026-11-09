using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Auth;

/// <summary>
/// DTO con la respuesta del inicio de sesion.
/// </summary>
[SwaggerSchema(Description = "Token JWT devuelto tras iniciar sesión correctamente.")]
public class LoginResponseDto {
    /// <summary>
    /// Token JWT que debe enviarse en la cabecera Authorization con el esquema Bearer.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9</example>
    [SwaggerSchema(Description = "Token JWT para la cabecera Authorization (esquema Bearer).")]
    public string Token { get; set; } = string.Empty;
}
