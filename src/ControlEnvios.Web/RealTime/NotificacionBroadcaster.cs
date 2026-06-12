using ControlEnvios.Application.Abstractions.RealTime;

namespace ControlEnvios.Web.RealTime;

/// <summary>
/// Mediador en memoria (singleton) que reparte avisos de notificación entre circuitos Blazor Server.
/// El circuito que cancela publica con <see cref="NotificarAsync"/>; las campanas de los proveedores
/// conectados están suscritas y se refrescan al instante por su propio circuito (SignalR), sin recargar.
/// </summary>
public sealed class NotificacionBroadcaster : INotificacionNotifier
{
    private readonly List<Func<string, Task>> _suscriptores = [];
    private readonly object _lock = new();

    public IDisposable Suscribir(Func<string, Task> handler)
    {
        lock (_lock)
        {
            _suscriptores.Add(handler);
        }
        return new Suscripcion(this, handler);
    }

    public async Task NotificarAsync(string codigoProveedor, CancellationToken ct = default)
    {
        Func<string, Task>[] copia;
        lock (_lock)
        {
            copia = [.. _suscriptores];
        }

        foreach (var handler in copia)
        {
            try
            {
                await handler(codigoProveedor);
            }
            catch
            {
                // Un suscriptor caído (circuito que se está cerrando) no debe afectar a los demás.
            }
        }
    }

    private void Quitar(Func<string, Task> handler)
    {
        lock (_lock)
        {
            _suscriptores.Remove(handler);
        }
    }

    private sealed class Suscripcion(NotificacionBroadcaster owner, Func<string, Task> handler) : IDisposable
    {
        public void Dispose() => owner.Quitar(handler);
    }
}
