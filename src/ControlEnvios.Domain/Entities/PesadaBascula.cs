namespace ControlEnvios.Domain.Entities;

/// <summary>
/// Pesada en báscula asociada (vincula el envío web con el pesaje real). Tabla legacy <c>PESADABASCULA</c>
/// (PK <c>ID_PESADA</c>). Origen del peso neto y de los flags de báscula grande/pequeña que ve el gestor.
/// </summary>
public class PesadaBascula
{
    public int Id { get; set; }
    public string? IdTicket { get; set; }
    public string? Matricula { get; set; }
    public string? Remolque { get; set; }

    /// <summary>(<c>COD_PROV</c>)</summary>
    public string? CodigoProveedor { get; set; }

    /// <summary>(<c>COD_CLIE</c>)</summary>
    public string? CodigoCliente { get; set; }

    /// <summary>(<c>COD_ART</c>)</summary>
    public int? CodigoArticulo { get; set; }

    public decimal? PrecioKg { get; set; }
    public DateTime? FechaEntrada { get; set; }
    public DateTime? HoraEntrada { get; set; }
    public DateTime? FechaSalida { get; set; }
    public DateTime? HoraSalida { get; set; }

    public decimal? Pesada1 { get; set; }
    public decimal? Pesada2 { get; set; }
    public decimal? PesadaNeto { get; set; }

    /// <summary>(<c>PRIMEPESADA</c>)</summary>
    public int? PrimeraPesada { get; set; }

    /// <summary>(<c>SECUNPESADA</c>)</summary>
    public int? SegundaPesada { get; set; }

    public string? Observaciones { get; set; }
    public string? UsuarioPesada { get; set; }
    public string? InfoModificacionPesada { get; set; }

    public int? EstadoCamion { get; set; }
    public int? EstadoMercancia { get; set; }

    /// <summary>Báscula grande. (<c>BALSAGRANDE</c>)</summary>
    public bool? BasculaGrande { get; set; }

    /// <summary>Báscula pequeña. (<c>BALSAPEQUENA</c>)</summary>
    public bool? BasculaPequena { get; set; }

    /// <summary>Vínculo con el envío web. (<c>IDENVIOWEB</c>)</summary>
    public int? IdEnvioWeb { get; set; }

    public bool? EmailEnviado { get; set; }

    /// <summary>Tara VGM. (<c>VGMTARA</c>)</summary>
    public decimal? VgmTara { get; set; }
}
