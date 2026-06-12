using ControlEnvios.Domain.Enums;

namespace ControlEnvios.Application.Autenticacion;

/// <summary>Resultado de un intento de autenticación.</summary>
public sealed record ResultadoLogin
{
    public bool Exito { get; init; }
    public RolUsuario? Rol { get; init; }

    /// <summary>Código de proveedor o alias de usuario autenticado.</summary>
    public string? Identificador { get; init; }
    public string? Nombre { get; init; }
    public string? Motivo { get; init; }

    public const string MensajeCredencialesInvalidas = "Usuario o contraseña incorrecta.";

    public static ResultadoLogin Correcto(RolUsuario rol, string identificador, string? nombre) =>
        new() { Exito = true, Rol = rol, Identificador = identificador, Nombre = nombre };

    public static ResultadoLogin Fallido() =>
        new() { Exito = false, Motivo = MensajeCredencialesInvalidas };
}
