using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiFunkosConjunta.Dto.Auth;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.IdentityModel.Tokens;

namespace ApiFunkosConjunta.Services.Implementations;

public class AuthService(IConfiguration configuration) : IAuthService {
    public Result<LoginResponseDto, DomainErrors> Login(LoginDto dto) {
        var username = configuration["Jwt:AdminUsername"];
        var password = configuration["Jwt:AdminPassword"];

        if (dto.Username != username || dto.Password != password)
            return Result.Failure<LoginResponseDto, DomainErrors>(new AuthErrors.InvalidCredentialsError());

        var claims = new[] {
            new Claim(ClaimTypes.Name, username!),
            new Claim(ClaimTypes.Role, "Administrador")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key, SecurityAlgorithms.HmacSha256);

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