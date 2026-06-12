namespace ControlEnvios.Domain.Entities;

/// <summary>
/// Catálogo de estados de mercancía. Tabla legacy <c>ESTADOMERCANCIA</c> (PK <c>ID</c>).
/// Fuente de verdad de los valores del enum <see cref="Enums.EstadoEnvio"/>.
/// </summary>
public class EstadoMercancia
{
    public int Id { get; set; }

    /// <summary>Descripción del estado. (columna legacy <c>ESTADOMERCANCIA1</c>)</summary>
    public string? Nombre { get; set; }
}
