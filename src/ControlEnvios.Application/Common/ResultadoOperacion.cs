namespace ControlEnvios.Application.Common;

/// <summary>Resultado simple de una operación de negocio (éxito/fallo con mensaje).</summary>
public sealed record ResultadoOperacion(bool Exito, string? Mensaje = null)
{
    public static ResultadoOperacion Ok(string? mensaje = null) => new(true, mensaje);
    public static ResultadoOperacion Error(string mensaje) => new(false, mensaje);
}
