namespace ControlEnvios.Application.Cupos;

public interface ICupoService
{
    /// <summary>
    /// Valida si un proveedor puede enviar <paramref name="kilosNuevoEnvio"/> kilos en la semana de
    /// <paramref name="fechaEnvio"/>, según su cupo asignado.
    /// </summary>
    Task<ResultadoCupo> ValidarEnvioAsync(string codigoProveedor, decimal kilosNuevoEnvio, DateOnly fechaEnvio, CancellationToken ct = default);
}
