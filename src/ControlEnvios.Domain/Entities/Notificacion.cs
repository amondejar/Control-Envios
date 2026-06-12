namespace ControlEnvios.Domain.Entities;

/// <summary>
/// Notificación in-app dirigida a un proveedor (p. ej. cancelación de un envío).
/// Vive en una tabla nueva propia (<c>NOTIFICACION</c>), no forma parte del esquema legacy.
/// </summary>
public class Notificacion
{
    public int Id { get; set; }

    /// <summary>Proveedor destinatario (<c>PROVEEDOR.CODPROVEEDOR</c>).</summary>
    public required string CodigoProveedor { get; set; }

    /// <summary>Tipo de notificación (p. ej. <c>CancelacionEnvio</c>).</summary>
    public string Tipo { get; set; } = string.Empty;

    public required string Titulo { get; set; }
    public string? Mensaje { get; set; }

    /// <summary>Envío relacionado, si aplica (<c>ENVIOMERCANCIA.IDENVIO</c>).</summary>
    public int? IdEnvio { get; set; }

    public DateTime FechaCreacion { get; set; }
    public bool Leida { get; set; }
    public DateTime? FechaLeida { get; set; }
}
