namespace ApiFunkosConjunta.Models;

public class Usuario {
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;
}