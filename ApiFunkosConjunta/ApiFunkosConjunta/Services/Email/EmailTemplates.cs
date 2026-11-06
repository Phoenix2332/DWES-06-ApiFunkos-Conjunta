namespace ApiFunkosConjunta.Services.Email;

/// <summary>
/// Plantillas de email para los eventos del catalogo de funkos.
/// </summary>
public static class EmailTemplates {
    /// <summary>
    /// Crea el HTML base sobre el que se monta el contenido de un email.
    /// </summary>
    public static string CreateBase(string title, string content) {
        return $@"<!DOCTYPE html>
<html lang='es'>
<head><meta charset='UTF-8'><title>{title}</title></head>
<body style='font-family: Arial, sans-serif; background: #f0f2f5; padding: 20px;'>
<div style='max-width: 600px; margin: 0 auto; background: #fff; border-radius: 12px; overflow: hidden;'>
    <div style='background: linear-gradient(135deg, #667eea, #764ba2); padding: 30px; text-align: center;'>
        <h1 style='color: #fff; margin: 0;'>ApiFunkos</h1>
    </div>
    <div style='padding: 30px;'>
        <h2 style='color: #1a1a2e; border-bottom: 2px solid #667eea; padding-bottom: 10px;'>{title}</h2>
        <div style='color: #4a4a68; line-height: 1.8;'>{content}</div>
    </div>
    <div style='background: #f8f9fa; padding: 20px; text-align: center; border-top: 1px solid #e9ecef;'>
        <p style='margin: 0; color: #6c757d; font-size: 12px;'>ApiFunkos - DAW</p>
    </div>
</div>
</body></html>";
    }

    /// <summary>
    /// Genera el contenido del email de aviso de un funko recien creado.
    /// </summary>
    public static string FunkoCreado(string nombre, decimal precio, int stock, int id) {
        return $@"
            <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
                <tr><td style='padding: 12px; background: #f8f9fa; font-weight: 600;'>ID:</td><td style='padding: 12px;'>{id}</td></tr>
                <tr><td style='padding: 12px; background: #f8f9fa; font-weight: 600;'>Nombre:</td><td style='padding: 12px;'>{nombre}</td></tr>
                <tr><td style='padding: 12px; background: #f8f9fa; font-weight: 600;'>Precio:</td><td style='padding: 12px; color: #28a745; font-weight: 600;'>{precio:N2} EUR</td></tr>
                <tr><td style='padding: 12px; background: #f8f9fa; font-weight: 600;'>Stock:</td><td style='padding: 12px;'>{stock} unidades</td></tr>
            </table>
            <p style='padding: 15px; background: #d4edda; border-left: 4px solid #28a745; border-radius: 4px;'>Funko creado correctamente.</p>";
    }
}
