namespace ControlEnvios.Infrastructure.Email;

/// <summary>Configuración SMTP (sección <c>Smtp</c> de la configuración). La contraseña va en User Secrets/entorno.</summary>
public sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 25;
    public bool EnableSsl { get; set; }
    public string From { get; set; } = string.Empty;
    public string? User { get; set; }
    public string? Password { get; set; }
}
