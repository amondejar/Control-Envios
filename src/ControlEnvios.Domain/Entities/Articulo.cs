namespace ControlEnvios.Domain.Entities;

/// <summary>Artículo/mercancía. Tabla legacy <c>ARTICULO</c> (PK <c>ID_ARTICULO</c>).</summary>
public class Articulo
{
    /// <summary>Identificador interno. Clave primaria. (<c>ID_ARTICULO</c>)</summary>
    public int Id { get; set; }

    /// <summary>Código de artículo usado en los envíos. (<c>CODARTICULO</c>)</summary>
    public required string Codigo { get; set; }

    public string? Descripcion { get; set; }

    /// <summary>Si el artículo está disponible para seleccionar en la web. (<c>ACTIVAWEB</c>)</summary>
    public bool? ActivoWeb { get; set; }
}
