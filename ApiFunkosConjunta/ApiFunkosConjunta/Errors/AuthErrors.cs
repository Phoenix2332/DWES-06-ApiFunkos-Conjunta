namespace ApiFunkosConjunta.Errors;

public abstract record AuthErrors(string Message) : DomainErrors(Message) {
    public sealed record InvalidCredentialsError()
        : AuthErrors("El usuario o la contraseña son incorrectos.");
}