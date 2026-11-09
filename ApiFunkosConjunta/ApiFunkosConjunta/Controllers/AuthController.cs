using ApiFunkosConjunta.Common.Examples;
using ApiFunkosConjunta.Common.Http;
using ApiFunkosConjunta.Dto.Auth;
using ApiFunkosConjunta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace ApiFunkosConjunta.Controllers;

/// <summary>
/// Endpoints REST de autenticacion.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[SwaggerTag("Autenticacion de los usuarios de la API mediante token JWT.")]
public class AuthController(IAuthService authService) : ControllerBase {
    /// <summary>
    /// Inicia sesion con nombre de usuario y contrasena y devuelve un token JWT.
    /// </summary>
    /// <param name="dto">Credenciales del usuario.</param>
    /// <returns>El token JWT y su fecha de caducidad.</returns>
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "Iniciar sesion",
        Description = "Comprueba las credenciales y devuelve un token JWT para el resto de endpoints.",
        OperationId = "Login"
    )]
    [SwaggerRequestExample(typeof(LoginDto), typeof(LoginDtoExample))]
    [SwaggerResponse(200, "Sesion iniciada correctamente", typeof(LoginResponseDto))]
    [SwaggerResponse(400, "Las credenciales no son validas", typeof(ValidationProblemDetails))]
    [SwaggerResponse(401, "El usuario o la contrasena son incorrectos", typeof(ProblemDetails))]
    [SwaggerResponse(500, "Error interno del servidor", typeof(ProblemDetails))]
    public async Task<IActionResult> Login(
        [FromBody, SwaggerRequestBody("Credenciales del usuario.", Required = true)] LoginDto dto) {
        var result = await authService.Login(dto);

        if (result.IsFailure)
            return Unauthorized(ApiProblem.Unauthorized(result.Error.Message, HttpContext?.Request.Path.Value));

        return Ok(result.Value);
    }
}
