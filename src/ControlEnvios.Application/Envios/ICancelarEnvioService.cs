using ControlEnvios.Application.Common;

namespace ControlEnvios.Application.Envios;

public interface ICancelarEnvioService
{
    /// <summary>
    /// Cancela un envío (estado → Cancelado) y genera una notificación in-app para el proveedor.
    /// Reemplaza el aviso por correo del legacy.
    /// </summary>
    Task<ResultadoOperacion> CancelarAsync(int idEnvio, CancellationToken ct = default);
}
