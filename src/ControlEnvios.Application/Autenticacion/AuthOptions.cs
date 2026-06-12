namespace ControlEnvios.Application.Autenticacion;

/// <summary>Opciones de autenticación (sección <c>Auth</c> de la configuración).</summary>
public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    /// <summary>
    /// Si está activo, al primer login válido se re-hashea la contraseña heredada (texto plano) a PBKDF2.
    /// </summary>
    /// <remarks>
    /// ⚠️ <b>Por defecto desactivado.</b> La BD <c>Bascula</c> es compartida con el sistema legacy de
    /// báscula, que valida contraseñas en texto plano. Re-hashear rompería el login del legacy. Activar
    /// solo en el corte (Fase 7), cuando el legacy ya no use la tabla y tras ampliar el ancho de las
    /// columnas <c>PASSWORD</c> (USUARIOS es <c>varchar(50)</c>, insuficiente para un hash PBKDF2).
    /// </remarks>
    public bool RehashPasswordsOnLogin { get; set; }
}
