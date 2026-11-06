using ApiFunkosConjunta.Common.Configuration;
using ApiFunkosConjunta.Dto.Funkos;
using ApiFunkosConjunta.Errors;
using ApiFunkosConjunta.Mappers;
using ApiFunkosConjunta.Models;
using ApiFunkosConjunta.Repositories.Interfaces;
using ApiFunkosConjunta.Services.Email;
using ApiFunkosConjunta.Services.Interfaces;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;

namespace ApiFunkosConjunta.Services.Implementations;

public class FunkoService(
    IFunkoRepository repository,
    IEmailService emailService,
    IOptions<SmtpSettings> smtpSettings,
    ILogger<FunkoService> logger) : IFunkoService {
    public async Task<Result<(IReadOnlyList<FunkoDto> Funkos, int Total), DomainErrors>> GetAllAsync(
        string? nombre, int pagina, int tamanoPagina, CancellationToken cancellationToken = default) {
        var result = await repository.GetAllAsync(nombre, pagina, tamanoPagina, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<(IReadOnlyList<FunkoDto>, int), DomainErrors>(result.Error);

        var funkos = result.Value.Funkos
            .Select(FunkoMapper.ToDto)
            .ToList();

        return Result.Success<(IReadOnlyList<FunkoDto>, int), DomainErrors>(
            (funkos, result.Value.Total));
    }

    public async Task<Result<FunkoDto, DomainErrors>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default) {
        var result = await repository.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(result.Error);

        return result.Value is null
            ? Result.Failure<FunkoDto, DomainErrors>(ServiceError.NotFoundError(id.ToString()))
            : Result.Success<FunkoDto, DomainErrors>(FunkoMapper.ToDto(result.Value));
    }

    public async Task<Result<FunkoDto, DomainErrors>> CreateAsync(CrearFunkoDto dto,
        CancellationToken cancellationToken = default) {
        var funko = FunkoMapper.ToModel(dto);

        var result = await repository.AddAsync(funko, cancellationToken);

        if (result.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(result.Error);

        // Aviso al administrador: el fallo del email no debe romper el alta del funko.
        await NotificarFunkoCreadoAsync(result.Value);

        return Result.Success<FunkoDto, DomainErrors>(FunkoMapper.ToDto(result.Value));
    }

    public async Task<Result<FunkoDto, DomainErrors>> UpdateAsync(int id, ActualizarFunkoDto dto,
        CancellationToken cancellationToken = default) {
        var getResult = await repository.GetByIdAsync(id, cancellationToken);

        if (getResult.IsFailure)
            return Result.Failure<FunkoDto, DomainErrors>(getResult.Error);

        if (getResult.Value is null)
            return Result.Failure<FunkoDto, DomainErrors>(ServiceError.NotFoundError(id.ToString()));

        var funko = getResult.Value;

        FunkoMapper.UpdateModel(funko, dto);

        var updateResult = await repository.UpdateAsync(funko, cancellationToken);

        return updateResult.IsFailure
            ? Result.Failure<FunkoDto, DomainErrors>(updateResult.Error)
            : Result.Success<FunkoDto, DomainErrors>(FunkoMapper.ToDto(updateResult.Value));
    }

    public async Task<Result<bool, DomainErrors>> DeleteAsync(int id, CancellationToken cancellationToken = default) {
        var getResult = await repository.GetByIdAsync(id, cancellationToken);

        if (getResult.IsFailure)
            return Result.Failure<bool, DomainErrors>(getResult.Error);

        if (getResult.Value is null)
            return Result.Failure<bool, DomainErrors>(ServiceError.NotFoundError(id.ToString()));

        return await repository.DeleteAsync(getResult.Value, cancellationToken);
    }

    public async Task<Result<bool, DomainErrors>> ExistsByCategoriaIdAsync(string categoriaId,
        CancellationToken cancellationToken = default) {
        return await repository.ExistsByCategoriaIdAsync(categoriaId, cancellationToken);
    }

    public async Task<Result<bool, DomainErrors>> ExistsByProveedorIdAsync(string proveedorId,
        CancellationToken cancellationToken = default) {
        return await repository.ExistsByProveedorIdAsync(proveedorId, cancellationToken);
    }

    /// <summary>
    /// Encola el aviso de nuevo funko para el administrador. Si el email falla, el alta ya esta
    /// hecha: se registra el aviso y no se propaga el error.
    /// </summary>
    private async Task NotificarFunkoCreadoAsync(Funko funko) {
        try {
            await emailService.EnqueueEmailAsync(new EmailMessage {
                To = smtpSettings.Value.AdminEmail,
                Subject = $"Nuevo funko: {funko.Nombre}",
                Body = EmailTemplates.CreateBase(
                    "Funko creado",
                    EmailTemplates.FunkoCreado(funko.Nombre, funko.Precio, funko.Stock, funko.Id)),
                IsHtml = true
            });
        } catch (Exception ex) {
            logger.LogWarning(ex, "Error encolando el aviso del funko {Id}", funko.Id);
        }
    }
}