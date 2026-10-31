namespace ApiFunkosConjunta.Errors;

public sealed record ApiErrors(string Message) : DomainErrors(Message);