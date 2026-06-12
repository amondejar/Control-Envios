namespace ControlEnvios.Infrastructure.Persistence.Consultas;

/// <summary>
/// Tipo de resultado (sin clave) del SP <c>LISTAENVIOSFECHAPROVEEDOR</c>. Los nombres de propiedad
/// coinciden con las columnas que devuelve el procedimiento.
/// </summary>
internal sealed class EnvioConsultaResultado
{
    public int Id { get; set; }            // columna "id"
    public string? NombreArticulo { get; set; }
    public string? Matricula { get; set; }
    public string? CodProveedor { get; set; }
    public string? NomProveedor { get; set; }
    public string? FechaEnvio { get; set; }
    public string? HoraEnvio { get; set; }
    public decimal KilosEnviados { get; set; }
    public int Estado { get; set; }
    public decimal? PesoBascula { get; set; }
    public bool? BasculaG { get; set; }
    public bool? BasculaP { get; set; }
    public bool Bascula3 { get; set; }   // SP: isnull(BALSA3,0) -> bit
    public string? HoraSalida { get; set; }
    public string? Observaciones { get; set; }
}
