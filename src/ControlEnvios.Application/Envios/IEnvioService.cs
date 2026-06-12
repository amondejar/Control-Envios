using ControlEnvios.Application.Common;
using ControlEnvios.Domain.Entities;

namespace ControlEnvios.Application.Envios;

public interface IEnvioService
{
    /// <summary>Crea un envío validando previamente el cupo del proveedor.</summary>
    Task<ResultadoOperacion> CrearAsync(CrearEnvioRequest request, CancellationToken ct = default);

    Task<ResultadoOperacion> EditarAsync(EditarEnvioRequest request, CancellationToken ct = default);

    Task<ResultadoOperacion> EliminarAsync(int id, CancellationToken ct = default);

    /// <summary>Envíos desde hoy en adelante.</summary>
    Task<IReadOnlyList<Envio>> ObtenerDesdeHoyAsync(CancellationToken ct = default);

    Task<Envio?> ObtenerPorIdAsync(int id, CancellationToken ct = default);
}
