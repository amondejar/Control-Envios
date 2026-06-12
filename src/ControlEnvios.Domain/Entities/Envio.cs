using ControlEnvios.Domain.Enums;

namespace ControlEnvios.Domain.Entities;

/// <summary>Envío de mercancía de un proveedor. Tabla legacy <c>ENVIOMERCANCIA</c> (PK <c>IDENVIO</c>).</summary>
public class Envio
{
    /// <summary>Identificador del envío. Clave primaria autogenerada. (<c>IDENVIO</c>)</summary>
    public int Id { get; set; }

    /// <summary>Código del proveedor que envía. (<c>CODPROVEEDOR</c>)</summary>
    public required string CodigoProveedor { get; set; }

    /// <summary>Código del artículo enviado. (<c>CODARTICULO</c>)</summary>
    public required string CodigoArticulo { get; set; }

    /// <summary>Fecha del envío. (<c>FECHAENVIO</c>)</summary>
    public DateOnly FechaEnvio { get; set; }

    /// <summary>Hora del envío. En el legacy se almacena en una columna <c>HORAENVIO</c> aparte. (nullable)</summary>
    public TimeOnly? HoraEnvio { get; set; }

    /// <summary>Kilos enviados declarados por el proveedor. (<c>KILOSENVIADOS</c>)</summary>
    public decimal KilosEnviados { get; set; }

    /// <summary>Estado del envío. (<c>ESTADO</c>)</summary>
    public EstadoEnvio Estado { get; set; }

    public string? Matricula { get; set; }
    public string? Observaciones { get; set; }

    /// <summary>Indica si debe notificarse por correo al proveedor. (<c>SENDEMAIL</c>)</summary>
    public bool EnviarEmail { get; set; }

    /// <summary>
    /// Indicador de si el envío tiene peso neto de báscula asociado. (<c>TIENENETO</c>, entero en el legacy).
    /// Confirmar semántica exacta en la Fase 3.
    /// </summary>
    public int TieneNeto { get; set; }
}
