using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Abstractions.Persistence;

public interface IUsuarioRepository
{
    /// <summary>Usuario interno por su alias de acceso. La verificación de credenciales se hace en el servicio.</summary>
    Task<Usuario?> GetByAliasAsync(string alias, CancellationToken ct = default);
}
