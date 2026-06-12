namespace ControlEnvios.Domain.Entities;

/// <summary>
/// Módulo del menú principal, mostrado u ocultado según el rol. Tabla legacy <c>MODULO</c> (PK <c>IdModulo</c>).
/// </summary>
public class Modulo
{
    public int Id { get; set; }
    public string? Nombre { get; set; }

    /// <summary>Acción MVC destino. (columna legacy con typo <c>Acction</c>)</summary>
    public string? Accion { get; set; }

    /// <summary>Controlador MVC destino. (<c>Controller</c>)</summary>
    public string? Controlador { get; set; }

    public string? Icono { get; set; }

    /// <summary>Nombre de la imagen de fondo. (<c>bgName</c>)</summary>
    public string? FondoNombre { get; set; }
}
