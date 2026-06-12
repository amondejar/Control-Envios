using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Abstractions.Persistence;

public interface ICupoRepository
{
    /// <summary>Cupo del proveedor si tiene control de cupo activo; <c>null</c> si no existe.</summary>
    Task<CupoProveedor?> GetCupoActivoProveedorAsync(string codigoProveedor, CancellationToken ct = default);

    /// <summary>Cupo global semanal en kilos (legacy: fila <c>CUPOKG.IDCUPOKG == 1</c>).</summary>
    Task<decimal> GetCupoGlobalKilosAsync(CancellationToken ct = default);

    /// <summary>
    /// Kilos enviados por el proveedor en la semana [inicio, fin].
    /// Equivale al SP legacy <c>KILOS_SEMANA_PROVEEDOR</c>.
    /// </summary>
    Task<decimal> GetKilosEnviadosSemanaAsync(DateOnly inicio, DateOnly fin, string codigoProveedor, CancellationToken ct = default);
}
