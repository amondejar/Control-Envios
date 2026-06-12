using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence.Repositories;

internal sealed class ArticuloRepository(BasculaDbContext context) : IArticuloRepository
{
    public async Task<IReadOnlyList<Articulo>> GetActivosWebAsync(CancellationToken ct = default) =>
        await context.Articulos
            .AsNoTracking()
            .Where(a => a.ActivoWeb == true)
            .OrderBy(a => a.Codigo)
            .ToListAsync(ct);
}
