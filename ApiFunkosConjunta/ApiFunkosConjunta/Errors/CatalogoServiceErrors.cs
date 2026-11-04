namespace ApiFunkosConjunta.Errors;

public abstract record CatalogoServiceErrors(string Message) : DomainErrors(Message) {
    public sealed record CategoriaConFunkosError(string Id)
        : CatalogoServiceErrors($"No se puede eliminar la categoría con id {Id} porque tiene Funkos asociados.");

    public sealed record ProveedorConFunkosError(string Id)
        : CatalogoServiceErrors($"No se puede eliminar el proveedor con id {Id} porque tiene Funkos asociados.");

    public sealed record ProveedorIdInvalidoError(string Id)
        : CatalogoServiceErrors($"El identificador del proveedor no tiene un formato válido: {Id}.");
}

public static class CatalogoServiceError {
    public static DomainErrors CategoriaConFunkosError(string id) {
        return new CatalogoServiceErrors.CategoriaConFunkosError(id);
    }

    public static DomainErrors ProveedorConFunkosError(string id) {
        return new CatalogoServiceErrors.ProveedorConFunkosError(id);
    }

    public static DomainErrors ProveedorIdInvalidoError(string id) {
        return new CatalogoServiceErrors.ProveedorIdInvalidoError(id);
    }
}