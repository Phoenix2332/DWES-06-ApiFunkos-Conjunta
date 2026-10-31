namespace ApiFunkosConjunta.Errors;

public abstract record RepositoryErrors(string Message) : DomainErrors(Message) {
    public sealed record CreationError()
        : RepositoryErrors("No se ha podido registrar la categoría en el sistema.");

    public sealed record UpdatedError()
        : RepositoryErrors("No se ha podido actualizar la categoría en el sistema.");

    public sealed record DeletedError()
        : RepositoryErrors("No se ha podido eliminar la categoría del sistema.");

    public sealed record GetAllError()
        : RepositoryErrors("No se han podido obtener las categorías.");

    public sealed record GetByIdError()
        : RepositoryErrors("No se ha podido obtener la categoría.");

    public sealed record ExistsByNameError()
        : RepositoryErrors("No se ha podido comprobar si existe una categoría con ese nombre.");
}

public static class RepositoryError {
    public static DomainErrors CreationError() {
        return new RepositoryErrors.CreationError();
    }

    public static DomainErrors UpdatedError() {
        return new RepositoryErrors.UpdatedError();
    }

    public static DomainErrors DeletedError() {
        return new RepositoryErrors.DeletedError();
    }

    public static DomainErrors GetAllError() {
        return new RepositoryErrors.GetAllError();
    }

    public static DomainErrors GetByIdError() {
        return new RepositoryErrors.GetByIdError();
    }

    public static DomainErrors ExistsByNameError() {
        return new RepositoryErrors.ExistsByNameError();
    }
}