using ApiFunkosConjunta.Dto.Auth;
using ApiFunkosConjunta.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiFunkosConjunta.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase {
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto) {
        var result = await authService.Login(dto);

        if (result.IsFailure)
            return Unauthorized(new { message = result.Error.Message });

        return Ok(result.Value);
    }
}