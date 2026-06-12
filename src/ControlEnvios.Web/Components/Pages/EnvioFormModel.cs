using System.ComponentModel.DataAnnotations;

namespace ControlEnvios.Web.Components.Pages;

/// <summary>Modelo de formulario para crear/editar un envío en la UI.</summary>
public sealed class EnvioFormModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Seleccione un artículo.")]
    public string? CodigoArticulo { get; set; }

    [Required(ErrorMessage = "Indique la fecha de envío.")]
    public DateTime? FechaEnvio { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Indique la hora (HH:mm).")]
    public string? HoraEnvio { get; set; } = DateTime.Now.ToString("HH:mm");

    [Required(ErrorMessage = "Indique los kilos enviados.")]
    [Range(0.0001, double.MaxValue, ErrorMessage = "Los kilos deben ser mayores que cero.")]
    public decimal? KilosEnviados { get; set; }

    [Required(ErrorMessage = "Indique la matrícula.")]
    public string? Matricula { get; set; }

    public string? Observaciones { get; set; }
}
