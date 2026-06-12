namespace ControlEnvios.Application.Consultas;

/// <summary>Fila de la consulta de envíos por fechas (resultado del SP <c>LISTAENVIOSFECHAPROVEEDOR</c>).</summary>
public sealed record ConsultaEnvio
{
    public int Id { get; init; }
    public string? NombreArticulo { get; init; }
    public string? Matricula { get; init; }
    public string? CodigoProveedor { get; init; }
    public string? NombreProveedor { get; init; }

    /// <summary>Fecha ya formateada por el SP (dd/MM/yyyy).</summary>
    public string? FechaEnvio { get; init; }
    public string? HoraEnvio { get; init; }
    public decimal KilosEnviados { get; init; }
    public int Estado { get; init; }

    /// <summary>Peso neto de báscula (de <c>PESADABASCULA</c>; nulo si el envío aún no se ha pesado).</summary>
    public decimal? PesoBascula { get; init; }
    public bool? BasculaGrande { get; init; }
    public bool? BasculaPequena { get; init; }
    public string? HoraSalida { get; init; }
    public string? Observaciones { get; init; }
}
