using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace ApiFunkosConjunta.Dto.Auth;

/// <summary>
/// DTO con las credenciales necesarias para iniciar sesion.
/// </summary>
[SwaggerSchema(Description = "Credenciales del usuario para iniciar sesión.")]
public class LoginDto {
    /// <summary>
    /// Nombre de usuario.
    /// </summary>
    /// <example>admin</example>
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [SwaggerSchema(Description = "Nombre de usuario.")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Contrasena del usuario.
    /// </summary>
    /// <example>Admin123!</example>
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [SwaggerSchema(Description = "Contraseña del usuario.")]
    public string Password { get; set; } = string.Empty;
}
