namespace ControlEnvios.Application.Common;

/// <summary>
/// Semana laboral de lunes a domingo a la que pertenece una fecha.
/// </summary>
/// <remarks>
/// Reemplaza el cálculo del legacy (<c>ObtenerPrimerdiaSemana</c>/<c>ObtenerUltimodiaSemana</c>),
/// que desplazaba la semana en domingo por usar <c>DayOfWeek</c> (Domingo = 0) sin normalizar.
/// Aquí el lunes es siempre el primer día, también en domingo.
/// </remarks>
public readonly record struct SemanaLaboral(DateOnly Inicio, DateOnly Fin)
{
    public static SemanaLaboral DeLaFecha(DateOnly fecha)
    {
        // DayOfWeek: Domingo = 0 … Sábado = 6. Normalizamos para que el lunes valga 0.
        int diasDesdeLunes = ((int)fecha.DayOfWeek + 6) % 7;
        var inicio = fecha.AddDays(-diasDesdeLunes);
        return new SemanaLaboral(inicio, inicio.AddDays(6));
    }
}
