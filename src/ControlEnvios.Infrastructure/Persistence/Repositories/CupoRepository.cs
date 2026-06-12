using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence.Repositories;

internal sealed class CupoRepository(BasculaDbContext context) : ICupoRepository
{
    public Task<CupoProveedor?> GetCupoActivoProveedorAsync(string codigoProveedor, CancellationToken ct = default) =>
        context.CuposProveedor
            .FirstOrDefaultAsync(c => c.CodigoProveedor == codigoProveedor && c.Activo, ct);

    public async Task<decimal> GetCupoGlobalKilosAsync(CancellationToken ct = default) =>
        await context.CupoGlobal
            .Where(c => c.Id == 1)
            .Select(c => c.CupoKilos)
            .FirstOrDefaultAsync(ct);

    public async Task<decimal> GetKilosEnviadosSemanaAsync(DateOnly inicio, DateOnly fin, string codigoProveedor, CancellationToken ct = default)
    {
        // SP legacy KILOS_SEMANA_PROVEEDOR(fechaInicio, fechaFin, codproveedor) -> escalar decimal.
        // TODO Fase 3 (con BD): validar que el SP devuelve una única columna mapeable a "Value";
        //   si no, alias en el SP o lectura por ADO. Parámetros como datetime (medianoche).
        var inicioDt = inicio.ToDateTime(TimeOnly.MinValue);
        var finDt = fin.ToDateTime(TimeOnly.MinValue);

        var resultado = await context.Database
            .SqlQueryRaw<decimal?>(
                "EXEC KILOS_SEMANA_PROVEEDOR @p0, @p1, @p2",
                inicioDt, finDt, codigoProveedor)
            .ToListAsync(ct);

        return resultado.FirstOrDefault() ?? 0m;
    }
}
