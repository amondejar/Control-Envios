using System.Security.Cryptography;
using ControlEnvios.Application.Abstractions.Security;

namespace ControlEnvios.Infrastructure.Security;

/// <summary>
/// Hash de contraseñas con PBKDF2 (HMAC-SHA256). Formato: <c>PBKDF2$iteraciones$saltB64$hashB64</c>.
/// </summary>
/// <remarks>
/// Soporta la <b>transición</b> desde el legacy: si el valor almacenado no tiene el prefijo del
/// formato, se interpreta como texto plano heredado y se compara directamente; en ese caso
/// <see cref="NecesitaRehash"/> devuelve <c>true</c> para que el login lo migre al primer acceso válido.
/// </remarks>
public sealed class Pbkdf2PasswordHasher : IPasswordHasher
{
    private const string Prefijo = "PBKDF2";
    private const int Iteraciones = 210_000;     // OWASP 2023 para PBKDF2-HMAC-SHA256
    private const int TamanoSalt = 16;           // 128 bits
    private const int TamanoHash = 32;           // 256 bits

    public string Hashear(string raw)
    {
        var salt = RandomNumberGenerator.GetBytes(TamanoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(raw, salt, Iteraciones, HashAlgorithmName.SHA256, TamanoHash);
        return $"{Prefijo}${Iteraciones}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string proporcionada, string? almacenada)
    {
        if (string.IsNullOrEmpty(almacenada))
        {
            return false;
        }

        // Valor heredado en texto plano (sin formato PBKDF2).
        if (!EsFormatoPbkdf2(almacenada))
        {
            return CryptographicOperations.FixedTimeEquals(
                System.Text.Encoding.UTF8.GetBytes(proporcionada),
                System.Text.Encoding.UTF8.GetBytes(almacenada));
        }

        var partes = almacenada.Split('$');
        if (partes.Length != 4 || !int.TryParse(partes[1], out var iteraciones))
        {
            return false;
        }

        var salt = Convert.FromBase64String(partes[2]);
        var hashEsperado = Convert.FromBase64String(partes[3]);
        var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(proporcionada, salt, iteraciones, HashAlgorithmName.SHA256, hashEsperado.Length);

        return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
    }

    public bool NecesitaRehash(string? almacenada)
    {
        if (string.IsNullOrEmpty(almacenada) || !EsFormatoPbkdf2(almacenada))
        {
            return true; // texto plano heredado o vacío → migrar
        }

        var partes = almacenada.Split('$');
        return partes.Length != 4 || !int.TryParse(partes[1], out var iteraciones) || iteraciones < Iteraciones;
    }

    private static bool EsFormatoPbkdf2(string valor) => valor.StartsWith(Prefijo + "$", StringComparison.Ordinal);
}
