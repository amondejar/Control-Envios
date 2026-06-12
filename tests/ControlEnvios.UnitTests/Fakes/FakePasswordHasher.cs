using ControlEnvios.Application.Abstractions.Security;

namespace ControlEnvios.UnitTests.Fakes;

/// <summary>Hasher de prueba con comparación en texto plano. <see cref="Rehash"/> simula necesidad de migración.</summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public bool Rehash { get; set; }

    public bool Verificar(string proporcionada, string? almacenada) => proporcionada == almacenada;
    public string Hashear(string raw) => $"HASHED:{raw}";
    public bool NecesitaRehash(string? almacenada) => Rehash;
}
