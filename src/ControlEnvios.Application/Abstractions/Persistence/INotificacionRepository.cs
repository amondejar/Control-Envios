using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Abstractions.Persistence;

public interface INotificacionRepository
{
    /// <summary>Añade una notificación (no confirma; usar <see cref="IUnitOfWork"/>).</summary>
    Task AddAsync(Notificacion notificacion, CancellationToken ct = default);

    Task<IReadOnlyList<Notificacion>> UltimasAsync(string codigoProveedor, int max = 20, CancellationToken ct = default);

    Task<int> ContarNoLeidasAsync(string codigoProveedor, CancellationToken ct = default);

    /// <summary>Marca como leídas todas las del proveedor (operación inmediata).</summary>
    Task MarcarTodasLeidasAsync(string codigoProveedor, CancellationToken ct = default);
}
