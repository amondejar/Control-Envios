namespace ControlEnvios.Domain.Entities;

/// <summary>
/// Cupo (porcentaje del cupo global semanal) asignado a un proveedor.
/// Tabla legacy <c>CUPOPROVEEDOR</c> (PK <c>IDCUPOPROV</c>).
/// </summary>
public class CupoProveedor
{
    /// <summary>Clave primaria. (<c>IDCUPOPROV</c>)</summary>
    public int Id { get; set; }

    /// <summary>Código del proveedor. (<c>CODPROVEE</c>)</summary>
    public required string CodigoProveedor { get; set; }

    /// <summary>Código de artículo al que aplica el cupo, si procede. (<c>CODARTICULO</c>)</summary>
    public string? CodigoArticulo { get; set; }

    public DateOnly? FechaInicio { get; set; }

    /// <summary>Fecha fin del cupo. (columna legacy con typo <c>FEHAFINALCUPO</c>)</summary>
    public DateOnly? FechaFin { get; set; }

    /// <summary>Porcentaje del cupo global semanal asignado al proveedor. (<c>PORCENTAGEASIG</c>)</summary>
    public decimal? PorcentajeAsignado { get; set; }

    /// <summary>Si el control de cupo está activo para este proveedor. (<c>ACTIVACUPO</c>)</summary>
    public bool Activo { get; set; }
}
