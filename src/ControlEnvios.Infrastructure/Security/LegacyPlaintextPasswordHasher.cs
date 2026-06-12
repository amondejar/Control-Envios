using ControlEnvios.Application.Abstractions.Security;

namespace ControlEnvios.Infrastructure.Security;

/// <summary>
/// Implementación de transición que conserva el comportamiento del legacy: las contraseñas se
/// almacenan y comparan en texto plano. Permite el login durante la migración.
/// </summary>
/// <remarks>
/// ⚠️ Inseguro. En la Fase 6 se sustituye por un hasher real (PBKDF2/<c>PasswordHasher&lt;T&gt;</c>)
/// con re-hash progresivo al primer login válido.
/// </remarks>
internal sealed class LegacyPlaintextPasswordHasher : IPasswordHasher
{
    public bool Verificar(string proporcionada, string? almacenada) =>
        almacenada is not null && string.Equals(proporcionada, almacenada, StringComparison.Ordinal);

    public string Hashear(string raw) => raw;
}
