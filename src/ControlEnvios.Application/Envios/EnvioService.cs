using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Application.Common;
using ControlEnvios.Application.Cupos;
using ControlEnvios.Domain.Entities;
using ControlEnvios.Domain.Enums;

namespace ControlEnvios.Application.Envios;

/// <summary>
/// Orquesta el alta/edición/borrado de envíos. Extrae la lógica de <c>EnviosController</c>, delegando
/// el control de cupo en <see cref="ICupoService"/> y eliminando la duplicación de los dos bloques
/// (con cupo / sin cupo) del legacy.
/// </summary>
public sealed class EnvioService(
    IEnvioRepository envioRepository,
    ICupoService cupoService,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : IEnvioService
{
    public async Task<ResultadoOperacion> CrearAsync(CrearEnvioRequest request, CancellationToken ct = default)
    {
        var cupo = await cupoService.ValidarEnvioAsync(request.CodigoProveedor, request.KilosEnviados, request.FechaEnvio, ct);
        if (!cupo.Permitido)
        {
            return ResultadoOperacion.Error(cupo.Motivo!);
        }

        var envio = new Envio
        {
            CodigoProveedor = request.CodigoProveedor,
            CodigoArticulo = request.CodigoArticulo,
            FechaEnvio = request.FechaEnvio,
            HoraEnvio = request.HoraEnvio,
            KilosEnviados = request.KilosEnviados,
            Estado = EstadoEnvio.Enviado,
            Matricula = request.Matricula,
            Observaciones = request.Observaciones ?? string.Empty,
            EnviarEmail = false,
            TieneNeto = 0,
        };

        await envioRepository.AddAsync(envio, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return ResultadoOperacion.Ok("Envío insertado correctamente.");
    }

    public async Task<ResultadoOperacion> EditarAsync(EditarEnvioRequest request, CancellationToken ct = default)
    {
        var envio = await envioRepository.GetByIdAsync(request.Id, ct);
        if (envio is null)
        {
            return ResultadoOperacion.Error("No existe el envío a actualizar.");
        }

        envio.CodigoProveedor = request.CodigoProveedor;
        envio.CodigoArticulo = request.CodigoArticulo;
        envio.FechaEnvio = request.FechaEnvio;
        envio.HoraEnvio = request.HoraEnvio;
        envio.KilosEnviados = request.KilosEnviados;
        envio.Matricula = request.Matricula;
        envio.Observaciones = request.Observaciones ?? string.Empty;

        envioRepository.Update(envio);
        await unitOfWork.SaveChangesAsync(ct);
        return ResultadoOperacion.Ok("Cambios realizados correctamente.");
    }

    public async Task<ResultadoOperacion> EliminarAsync(int id, CancellationToken ct = default)
    {
        var envio = await envioRepository.GetByIdAsync(id, ct);
        if (envio is null)
        {
            return ResultadoOperacion.Error("No existe el envío a eliminar.");
        }

        envioRepository.Remove(envio);
        await unitOfWork.SaveChangesAsync(ct);
        return ResultadoOperacion.Ok("Envío eliminado correctamente.");
    }

    public Task<IReadOnlyList<Envio>> ObtenerDesdeHoyAsync(CancellationToken ct = default)
    {
        var hoy = DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime);
        return envioRepository.GetDesdeFechaAsync(hoy, ct);
    }

    public Task<Envio?> ObtenerPorIdAsync(int id, CancellationToken ct = default) =>
        envioRepository.GetByIdAsync(id, ct);
}
