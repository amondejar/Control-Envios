using ControlEnvios.Application.Abstractions.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ControlEnvios.Infrastructure.Email;

/// <summary>
/// Envío de correo por SMTP con MailKit. Porta el proyecto legacy <c>SmptEmailSendProveedor</c>
/// (que usaba <c>System.Net.Mail</c> con credenciales hardcodeadas) a configuración externa.
/// </summary>
internal sealed class SmtpEmailService(IOptions<SmtpOptions> options) : IEmailService
{
    private readonly SmtpOptions _options = options.Value;

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpo, CancellationToken ct = default)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(MailboxAddress.Parse(_options.From));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;
        mensaje.Body = new TextPart("plain") { Text = cuerpo };

        using var cliente = new SmtpClient();
        var seguridad = _options.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await cliente.ConnectAsync(_options.Host, _options.Port, seguridad, ct);

        if (!string.IsNullOrEmpty(_options.User))
        {
            await cliente.AuthenticateAsync(_options.User, _options.Password ?? string.Empty, ct);
        }

        await cliente.SendAsync(mensaje, ct);
        await cliente.DisconnectAsync(quit: true, ct);
    }
}
