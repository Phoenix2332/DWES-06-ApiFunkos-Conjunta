using ApiFunkosConjunta.Dto.Auth;
using ApiFunkosConjunta.Errors;
using CSharpFunctionalExtensions;

namespace ApiFunkosConjunta.Services.Interfaces;

public interface IAuthService {
    Result<LoginResponseDto, DomainErrors> Login(LoginDto dto);
}