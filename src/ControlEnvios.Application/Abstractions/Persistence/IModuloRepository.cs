using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Abstractions.Persistence;

public interface IModuloRepository
{
    /// <summary>Todos los módulos. El filtrado por rol es lógica de negocio (servicio).</summary>
    Task<IReadOnlyList<Modulo>> GetTodosAsync(CancellationToken ct = default);
}
