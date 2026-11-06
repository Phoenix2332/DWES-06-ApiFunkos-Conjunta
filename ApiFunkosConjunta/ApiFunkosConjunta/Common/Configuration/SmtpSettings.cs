namespace ApiFunkosConjunta.Common.Configuration;

/// <summary>
/// Configuracion SMTP tipada (seccion "Smtp" de appsettings.json o variables de entorno).
/// </summary>
public class SmtpSettings {
    /// <summary>Servidor SMTP.</summary>
    public string Host { get; set; } = "localhost";

    /// <summary>Puerto del servidor SMTP.</summary>
    public int Port { get; set; } = 587;

    /// <summary>Usuario de autenticacion. Vacio = sin autenticacion.</summary>
    public string? Username { get; set; }

    /// <summary>Contrasena de autenticacion.</summary>
    public string? Password { get; set; }

    /// <summary>Direccion remitente.</summary>
    public string FromEmail { get; set; } = "noreply@apifunkos.com";

    /// <summary>Nombre que se muestra como remitente.</summary>
    public string FromName { get; set; } = "ApiFunkos";

    /// <summary>Destinatario de los avisos de aviso (por ejemplo, el administrador).</summary>
    public string AdminEmail { get; set; } = "admin@apifunkos.com";

    /// <summary>Seguridad de la conexion: Auto, Tls, Ssl o None.</summary>
    public string Security { get; set; } = "Auto";
}
