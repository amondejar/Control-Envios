namespace ControlEnvios.Application.Consultas;

public interface IConsultaEnviosService
{
    /// <summary>
    /// Consulta de envíos por rango de fechas, con filtros opcionales de proveedor y artículo
    /// (SP legacy <c>LISTAENVIOSFECHAPROVEEDOR</c>).
    /// </summary>
    Task<IReadOnlyList<ConsultaEnvio>> PorFechasAsync(
        DateOnly inicio, DateOnly fin, string? codigoProveedor, string? codigoArticulo, CancellationToken ct = default);
}
