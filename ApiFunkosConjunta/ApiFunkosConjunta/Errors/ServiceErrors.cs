namespace ApiFunkosConjunta.Errors;

public abstract record ServiceErrors(string Message) : DomainErrors(Message) {
    public sealed record NotFoundError(string Id)
        : ServiceErrors($"No se ha encontrado la categoría con el id: {Id}.");

    public sealed record GetAllError()
        : ServiceErrors("Error al obtener las categorías.");

    public sealed record GetByIdError(string Id)
        : ServiceErrors($"Error al obtener la categoría con el id {Id}.");

    public sealed record CreateError()
        : ServiceErrors("Error al crear la categoría.");

    public sealed record DuplicateNameError(string Nombre)
        : ServiceErrors($"Ya existe una categoría con el nombre: {Nombre}.");

    public sealed record UpdateError(string Id)
        : ServiceErrors($"Error al actualizar la categoría con el id {Id}.");

    public sealed record DeleteError(string Id)
        : ServiceErrors($"Error al eliminar la categoría con el id {Id}.");
}

public static class ServiceError {
    public static DomainErrors NotFoundError(string id) {
        return new ServiceErrors.NotFoundError(id);
    }

    public static DomainErrors GetAllError() {
        return new ServiceErrors.GetAllError();
    }

    public static DomainErrors GetByIdError(string id) {
        return new ServiceErrors.GetByIdError(id);
    }

    public static DomainErrors CreateError() {
        return new ServiceErrors.CreateError();
    }

    public static DomainErrors DuplicateNameError(string nombre) {
        return new ServiceErrors.DuplicateNameError(nombre);
    }

    public static DomainErrors UpdateError(string id) {
        return new ServiceErrors.UpdateError(id);
    }

    public static DomainErrors DeleteError(string id) {
        return new ServiceErrors.DeleteError(id);
    }
}