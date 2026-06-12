using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence.Repositories;

internal sealed class ModuloRepository(BasculaDbContext context) : IModuloRepository
{
    public async Task<IReadOnlyList<Modulo>> GetTodosAsync(CancellationToken ct = default) =>
        await context.Modulos.AsNoTracking().OrderBy(m => m.Id).ToListAsync(ct);
}
