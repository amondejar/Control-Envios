using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Domain.Entities;

namespace ControlEnvios.UnitTests.Fakes;

internal sealed class FakeNotificacionRepository : INotificacionRepository
{
    public List<Notificacion> Items { get; } = [];

    public Task AddAsync(Notificacion notificacion, CancellationToken ct = default)
    {
        Items.Add(notificacion);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Notificacion>> UltimasAsync(string codigoProveedor, int max = 20, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<Notificacion>>(Items.Where(n => n.CodigoProveedor == codigoProveedor).ToList());

    public Task<int> ContarNoLeidasAsync(string codigoProveedor, CancellationToken ct = default) =>
        Task.FromResult(Items.Count(n => n.CodigoProveedor == codigoProveedor && !n.Leida));

    public Task MarcarTodasLeidasAsync(string codigoProveedor, CancellationToken ct = default)
    {
        foreach (var n in Items.Where(n => n.CodigoProveedor == codigoProveedor))
        {
            n.Leida = true;
        }
        return Task.CompletedTask;
    }
}
