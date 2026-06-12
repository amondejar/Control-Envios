using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence.Repositories;

internal sealed class EnvioRepository(BasculaDbContext context) : IEnvioRepository
{
    public Task<Envio?> GetByIdAsync(int id, CancellationToken ct = default) =>
        context.Envios.FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyList<Envio>> GetDesdeFechaAsync(DateOnly desde, CancellationToken ct = default) =>
        await context.Envios
            .AsNoTracking()
            .Where(e => e.FechaEnvio >= desde)
            .OrderBy(e => e.FechaEnvio)
            .ToListAsync(ct);

    public async Task AddAsync(Envio envio, CancellationToken ct = default) =>
        await context.Envios.AddAsync(envio, ct);

    public void Update(Envio envio) => context.Envios.Update(envio);

    public void Remove(Envio envio) => context.Envios.Remove(envio);
}
