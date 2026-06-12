namespace ControlEnvios.Application.Envios;

/// <summary>Datos para crear un envío.</summary>
public sealed record CrearEnvioRequest(
    string CodigoProveedor,
    string CodigoArticulo,
    DateOnly FechaEnvio,
    TimeOnly? HoraEnvio,
    decimal KilosEnviados,
    string? Matricula,
    string? Observaciones);

/// <summary>Datos para editar un envío existente.</summary>
public sealed record EditarEnvioRequest(
    int Id,
    string CodigoProveedor,
    string CodigoArticulo,
    DateOnly FechaEnvio,
    TimeOnly? HoraEnvio,
    decimal KilosEnviados,
    string? Matricula,
    string? Observaciones);
