using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Abstractions.Persistence;

public interface IProveedorRepository
{
    Task<Proveedor?> GetByCodigoAsync(string codigo, CancellationToken ct = default);

    /// <summary>Proveedores seleccionables en los filtros del gestor (legacy: <c>CODPOSTAL != "z"</c>).</summary>
    Task<IReadOnlyList<Proveedor>> GetSeleccionablesAsync(CancellationToken ct = default);
}
