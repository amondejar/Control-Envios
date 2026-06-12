using ControlEnvios.Application.Abstractions.Persistence;
using ControlEnvios.Application.Common;

namespace ControlEnvios.Application.Cupos;

/// <summary>
/// Reglas de cupo semanal por proveedor. Extrae la lógica de <c>EnviosController.Create</c> del legacy.
/// </summary>
/// <remarks>
/// Reglas (caracterización en docs/FASE1-CARACTERIZACION.md §1.3):
/// <list type="number">
/// <item>Sin cupo activo → siempre permitido.</item>
/// <item>Con cupo activo y es el <b>primer envío de la semana</b> (0 kg enviados) → permitido aunque exceda
/// (regla de negocio heredada, conservada).</item>
/// <item>Con cupo activo y no es el primero → permitido si
/// <c>kilosSemana + kilosNuevoEnvio ≤ (cupoGlobal × porcentaje / 100)</c>.</item>
/// </list>
/// Frente al legacy se corrige: el cálculo de la semana (lunes–domingo correcto) y el posible
/// <c>NullReferenceException</c> cuando el proveedor no tiene fila de cupo.
/// </remarks>
public sealed class CupoService(ICupoRepository cupoRepository) : ICupoService
{
    public async Task<ResultadoCupo> ValidarEnvioAsync(string codigoProveedor, decimal kilosNuevoEnvio, DateOnly fechaEnvio, CancellationToken ct = default)
    {
        var cupo = await cupoRepository.GetCupoActivoProveedorAsync(codigoProveedor, ct);

        // Sin cupo activo o sin porcentaje asignado → sin restricción.
        if (cupo is null || !cupo.Activo || cupo.PorcentajeAsignado is null)
        {
            return ResultadoCupo.Ok();
        }

        var semana = SemanaLaboral.DeLaFecha(fechaEnvio);
        var kilosSemana = await cupoRepository.GetKilosEnviadosSemanaAsync(semana.Inicio, semana.Fin, codigoProveedor, ct);

        // Primer envío de la semana: permitido (regla heredada).
        if (kilosSemana <= 0)
        {
            return ResultadoCupo.Ok(kilosSemana: 0);
        }

        var cupoGlobal = await cupoRepository.GetCupoGlobalKilosAsync(ct);
        var kilosAsignados = cupoGlobal * cupo.PorcentajeAsignado.Value / 100m;

        return kilosSemana + kilosNuevoEnvio <= kilosAsignados
            ? ResultadoCupo.Ok(kilosAsignados, kilosSemana)
            : ResultadoCupo.Excedido(kilosAsignados, kilosSemana);
    }
}
