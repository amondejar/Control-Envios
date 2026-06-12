using ControlEnvios.Application.Abstractions.Security;

namespace ControlEnvios.UnitTests.Fakes;

/// <summary>Hasher de prueba con comparación en texto plano (igual que el legacy).</summary>
internal sealed class FakePasswordHasher : IPasswordHasher
{
    public bool Verificar(string proporcionada, string? almacenada) => proporcionada == almacenada;
    public string Hashear(string raw) => raw;
}
