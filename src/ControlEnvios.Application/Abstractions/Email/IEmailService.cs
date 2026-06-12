namespace ControlEnvios.Application.Abstractions.Email;

/// <summary>Envío de notificaciones por correo a proveedores. Reemplaza el proyecto legacy <c>SmptEmailSendProveedor</c>.</summary>
public interface IEmailService
{
    Task EnviarAsync(string destinatario, string asunto, string cuerpo, CancellationToken ct = default);
}
