namespace ControlEnvios.Domain.Entities;

/// <summary>
/// Cupo global semanal en kilos del que se reparten los porcentajes por proveedor.
/// Tabla legacy <c>CUPOKG</c> (PK <c>IDCUPOKG</c>). El legacy usa la fila <c>IDCUPOKG == 1</c>.
/// </summary>
public class CupoGlobal
{
    /// <summary>Clave primaria. (<c>IDCUPOKG</c>)</summary>
    public int Id { get; set; }

    /// <summary>Cupo total semanal en kilos. (<c>CUPOKILOS</c>)</summary>
    public decimal CupoKilos { get; set; }
}
