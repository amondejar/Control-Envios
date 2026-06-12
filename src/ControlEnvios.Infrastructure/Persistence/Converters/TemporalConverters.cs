using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ControlEnvios.Infrastructure.Persistence.Converters;

/// <summary>
/// Conversores entre los tipos modernos del dominio (<see cref="DateOnly"/>/<see cref="TimeOnly"/>) y
/// las columnas <c>datetime</c> del esquema legacy (que almacena fecha y hora por separado en columnas
/// de tipo datetime). Permite conservar el dominio limpio sin tocar el esquema.
/// </summary>
public sealed class DateOnlyToDateTimeConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyToDateTimeConverter()
        : base(d => d.ToDateTime(TimeOnly.MinValue), dt => DateOnly.FromDateTime(dt))
    {
    }
}

public sealed class TimeOnlyToDateTimeConverter : ValueConverter<TimeOnly, DateTime>
{
    // Fecha base arbitraria (válida para SQL 'datetime'); solo importa la parte de hora.
    private static readonly DateTime Base = new(1900, 1, 1);

    public TimeOnlyToDateTimeConverter()
        : base(t => Base.Add(t.ToTimeSpan()), dt => TimeOnly.FromDateTime(dt))
    {
    }
}
