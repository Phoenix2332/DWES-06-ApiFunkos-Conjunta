using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiFunkosConjunta.Data;
using ApiFunkosConjunta.Dto.Auth;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ApiFunkosConjunta.Services.Implementations;

public class AuthService(AppDbContext context, IConfiguration configuration) : IAuthService {
    public async Task<Result<LoginResponseDto, DomainErrors>> Login(LoginDto dto) {
        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == dto.Username);

        if (usuario is null)
            return Result.Failure<LoginResponseDto, DomainErrors>(new AuthErrors.InvalidCredentialsError());

        var passwordHasher = new PasswordHasher<Usuario>();

        var resultadoPassword = passwordHasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password);

        if (resultadoPassword == PasswordVerificationResult.Failed)
            return Result.Failure<LoginResponseDto, DomainErrors>(new AuthErrors.InvalidCredentialsError());

        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreUsuario),
            new Claim(ClaimTypes.Role, usuario.Rol)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return Result.Success<LoginResponseDto, DomainErrors>(
            new LoginResponseDto {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
    }
}