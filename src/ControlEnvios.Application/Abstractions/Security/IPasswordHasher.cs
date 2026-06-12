namespace ControlEnvios.Application.Abstractions.Security;

/// <summary>
/// Verificación y generación de hash de contraseñas. En el legacy las contraseñas son texto plano;
/// la implementación actual conserva esa semántica para permitir el login durante la migración.
/// La Fase 6 sustituye la implementación por un hash real (PBKDF2) con re-hash progresivo.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Comprueba si la contraseña proporcionada corresponde al valor almacenado.</summary>
    bool Verificar(string proporcionada, string? almacenada);

    /// <summary>Genera el valor a almacenar para una contraseña.</summary>
    string Hashear(string raw);
}
