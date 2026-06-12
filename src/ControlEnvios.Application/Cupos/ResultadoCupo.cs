namespace ControlEnvios.Application.Cupos;

/// <summary>Resultado de validar si un envío cabe dentro del cupo del proveedor.</summary>
public sealed record ResultadoCupo
{
    public bool Permitido { get; init; }
    public string? Motivo { get; init; }

    /// <summary>Kilos asignados al proveedor esta semana según su porcentaje (informativo).</summary>
    public decimal? KilosAsignados { get; init; }

    /// <summary>Kilos ya enviados por el proveedor esta semana (informativo).</summary>
    public decimal? KilosSemana { get; init; }

    public const string MensajeCupoExcedido = "Cupo excedido, por favor contacte con el responsable de compras.";

    public static ResultadoCupo Ok(decimal? kilosAsignados = null, decimal? kilosSemana = null) =>
        new() { Permitido = true, KilosAsignados = kilosAsignados, KilosSemana = kilosSemana };

    public static ResultadoCupo Excedido(decimal kilosAsignados, decimal kilosSemana) =>
        new() { Permitido = false, Motivo = MensajeCupoExcedido, KilosAsignados = kilosAsignados, KilosSemana = kilosSemana };
}
