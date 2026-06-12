using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControlEnvios.Infrastructure.Persistence.Repositories;

internal sealed class NotificacionRepository(BasculaDbContext context, TimeProvider clock) : INotificacionRepository
{
    public async Task AddAsync(Notificacion notificacion, CancellationToken ct = default) =>
        await context.Notificaciones.AddAsync(notificacion, ct);

    public async Task<IReadOnlyList<Notificacion>> UltimasAsync(string codigoProveedor, int max = 20, CancellationToken ct = default) =>
        await context.Notificaciones
            .AsNoTracking()
            .Where(n => n.CodigoProveedor == codigoProveedor)
            .OrderByDescending(n => n.FechaCreacion)
            .Take(max)
            .ToListAsync(ct);

    public Task<int> ContarNoLeidasAsync(string codigoProveedor, CancellationToken ct = default) =>
        context.Notificaciones.CountAsync(n => n.CodigoProveedor == codigoProveedor && !n.Leida, ct);

    public Task MarcarTodasLeidasAsync(string codigoProveedor, CancellationToken ct = default) =>
        context.Notificaciones
            .Where(n => n.CodigoProveedor == codigoProveedor && !n.Leida)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Leida, true)
                .SetProperty(n => n.FechaLeida, clock.GetLocalNow().DateTime), ct);
}
