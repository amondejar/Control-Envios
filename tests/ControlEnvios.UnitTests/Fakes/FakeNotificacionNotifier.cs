using ControlEnvios.Application.Abstractions.RealTime;

namespace ControlEnvios.UnitTests.Fakes;

internal sealed class FakeNotificacionNotifier : INotificacionNotifier
{
    public List<string> Notificados { get; } = [];

    public Task NotificarAsync(string codigoProveedor, CancellationToken ct = default)
    {
        Notificados.Add(codigoProveedor);
        return Task.CompletedTask;
    }
}
