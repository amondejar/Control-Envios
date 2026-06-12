using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Application.Common;
using ControlEnvios.Domain.Entities;
using ControlEnvios.Domain.Enums;

namespace ControlEnvios.Application.Envios;

/// <summary>
/// Cancela un envío y avisa al proveedor mediante una notificación in-app (sin email).
/// La actualización del estado y el alta de la notificación se confirman en una sola transacción.
/// </summary>
public sealed class CancelarEnvioService(
    IEnvioRepository envioRepository,
    INotificacionRepository notificacionRepository,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider) : ICancelarEnvioService
{
    public async Task<ResultadoOperacion> CancelarAsync(int idEnvio, CancellationToken ct = default)
    {
        var envio = await envioRepository.GetByIdAsync(idEnvio, ct);
        if (envio is null)
        {
            return ResultadoOperacion.Error("No existe el envío a cancelar.");
        }

        if (envio.Estado == EstadoEnvio.Cancelado)
        {
            return ResultadoOperacion.Error("El envío ya estaba cancelado.");
        }

        envio.Estado = EstadoEnvio.Cancelado;
        envioRepository.Update(envio);

        var matricula = string.IsNullOrWhiteSpace(envio.Matricula) ? string.Empty : $" (matrícula {envio.Matricula})";
        await notificacionRepository.AddAsync(new Notificacion
        {
            CodigoProveedor = envio.CodigoProveedor,
            Tipo = "CancelacionEnvio",
            Titulo = "Envío cancelado",
            Mensaje = $"Su envío del {envio.FechaEnvio:dd/MM/yyyy}{matricula} ha sido cancelado por el gestor.",
            IdEnvio = envio.Id,
            FechaCreacion = timeProvider.GetLocalNow().DateTime,
            Leida = false,
        }, ct);

        await unitOfWork.SaveChangesAsync(ct);
        return ResultadoOperacion.Ok("Envío cancelado y proveedor notificado.");
    }
}
