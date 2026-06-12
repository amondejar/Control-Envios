using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence.Repositories;

internal sealed class ProveedorRepository(BasculaDbContext context) : IProveedorRepository
{
    public Task<Proveedor?> GetByCodigoAsync(string codigo, CancellationToken ct = default) =>
        context.Proveedores.FirstOrDefaultAsync(p => p.Codigo == codigo, ct);

    public async Task<IReadOnlyList<Proveedor>> GetSeleccionablesAsync(CancellationToken ct = default) =>
        await context.Proveedores
            .AsNoTracking()
            .Where(p => p.CodigoPostal != "z")
            .OrderBy(p => p.Codigo)
            .ToListAsync(ct);
}
