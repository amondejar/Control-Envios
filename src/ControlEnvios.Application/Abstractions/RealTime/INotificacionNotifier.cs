namespace ControlEnvios.Application.Abstractions.RealTime;

/// <summary>
/// Publica en tiempo real que un proveedor tiene una notificación nueva, para que su UI se actualice
/// sin recargar. En Blazor Server se implementa sobre el propio circuito (SignalR).
/// </summary>
public interface INotificacionNotifier
{
    Task NotificarAsync(string codigoProveedor, CancellationToken ct = default);
}
