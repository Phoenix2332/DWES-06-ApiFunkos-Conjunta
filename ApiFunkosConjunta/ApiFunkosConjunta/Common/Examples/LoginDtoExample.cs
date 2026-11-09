using ApiFunkosConjunta.Dto.Auth;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Common.Examples;

/// <summary>
/// Ejemplo de peticion para iniciar sesion (apartado 24.8 del tema).
/// </summary>
public class LoginDtoExample : IExamplesProvider<LoginDto> {
    /// <summary>
    /// Devuelve unas credenciales de ejemplo que Swagger UI muestra como cuerpo de la peticion.
    /// </summary>
    public LoginDto GetExamples() {
        return new LoginDto {
            Username = "admin",
            Password = "Admin123!"
        };
    }
}
