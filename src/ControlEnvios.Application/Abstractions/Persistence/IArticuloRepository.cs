using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Abstractions.Persistence;

public interface IArticuloRepository
{
    /// <summary>Artículos activos para la web (legacy: <c>ACTIVAWEB == true</c>), ordenados por código.</summary>
    Task<IReadOnlyList<Articulo>> GetActivosWebAsync(CancellationToken ct = default);
}
