namespace ControlEnvios.Domain.Entities;

/// <summary>Proveedor que realiza envíos de mercancía. Tabla legacy <c>PROVEEDOR</c> (PK <c>CODPROVEEDOR</c>).</summary>
public class Proveedor
{
    /// <summary>Código de proveedor. Clave primaria. (<c>CODPROVEEDOR</c>)</summary>
    public required string Codigo { get; set; }

    public string? Nombre { get; set; }
    public string? Cif { get; set; }
    public string? Direccion { get; set; }
    public string? Poblacion { get; set; }
    public string? Provincia { get; set; }
    public string? CodigoPostal { get; set; }
    public string? Observaciones { get; set; }
    public string? Email { get; set; }

    /// <summary>
    /// Contraseña en texto plano en el legacy (<c>PASSWORD</c>).
    /// En la Fase 6 se sustituye por un hash (PBKDF2/<c>PasswordHasher</c>).
    /// </summary>
    public string? Password { get; set; }
}
