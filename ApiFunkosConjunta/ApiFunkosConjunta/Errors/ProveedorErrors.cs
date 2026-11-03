namespace ApiFunkosConjunta.Errors;

public abstract record ProveedorServiceErrors(string Message) : DomainErrors(Message) {
    public sealed record NotFoundError(string Id)
        : ProveedorServiceErrors($"No se ha encontrado el proveedor con el id: {Id}.");

    public sealed record GetAllError()
        : ProveedorServiceErrors("Error al obtener los proveedores.");

    public sealed record GetByIdError(string Id)
        : ProveedorServiceErrors($"Error al obtener el proveedor con el id {Id}.");

    public sealed record CreateError()
        : ProveedorServiceErrors("Error al crear el proveedor.");

    public sealed record DuplicateNameError(string Nombre)
        : ProveedorServiceErrors($"Ya existe un proveedor con el nombre: {Nombre}.");

    public sealed record UpdateError(string Id)
        : ProveedorServiceErrors($"Error al actualizar el proveedor con el id {Id}.");

    public sealed record DeleteError(string Id)
        : ProveedorServiceErrors($"Error al eliminar el proveedor con el id {Id}.");
}

public static class ProveedorServiceError {
    public static DomainErrors NotFoundError(string id) {
        return new ProveedorServiceErrors.NotFoundError(id);
    }

    public static DomainErrors GetAllError() {
        return new ProveedorServiceErrors.GetAllError();
    }

    public static DomainErrors GetByIdError(string id) {
        return new ProveedorServiceErrors.GetByIdError(id);
    }

    public static DomainErrors CreateError() {
        return new ProveedorServiceErrors.CreateError();
    }

    public static DomainErrors DuplicateNameError(string nombre) {
        return new ProveedorServiceErrors.DuplicateNameError(nombre);
    }

    public static DomainErrors UpdateError(string id) {
        return new ProveedorServiceErrors.UpdateError(id);
    }

    public static DomainErrors DeleteError(string id) {
        return new ProveedorServiceErrors.DeleteError(id);
    }
}

public abstract record ProveedorRepositoryErrors(string Message) : DomainErrors(Message) {
    public sealed record CreationError()
        : ProveedorRepositoryErrors("No se ha podido registrar el proveedor en el sistema.");

    public sealed record UpdatedError()
        : ProveedorRepositoryErrors("No se ha podido actualizar el proveedor en el sistema.");

    public sealed record DeletedError()
        : ProveedorRepositoryErrors("No se ha podido eliminar el proveedor del sistema.");

    public sealed record GetAllError()
        : ProveedorRepositoryErrors("No se han podido obtener los proveedores.");

    public sealed record GetByIdError()
        : ProveedorRepositoryErrors("No se ha podido obtener el proveedor.");

    public sealed record ExistsByNameError()
        : ProveedorRepositoryErrors("No se ha podido comprobar si existe un proveedor con ese nombre.");
}

public static class ProveedorRepositoryError {
    public static DomainErrors CreationError() {
        return new ProveedorRepositoryErrors.CreationError();
    }

    public static DomainErrors UpdatedError() {
        return new ProveedorRepositoryErrors.UpdatedError();
    }

    public static DomainErrors DeletedError() {
        return new ProveedorRepositoryErrors.DeletedError();
    }

    public static DomainErrors GetAllError() {
        return new ProveedorRepositoryErrors.GetAllError();
    }

    public static DomainErrors GetByIdError() {
        return new ProveedorRepositoryErrors.GetByIdError();
    }

    public static DomainErrors ExistsByNameError() {
        return new ProveedorRepositoryErrors.ExistsByNameError();
    }
}
