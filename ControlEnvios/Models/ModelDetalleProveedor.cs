using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlEnvios.Models
{
    public class ModelDetalleProveedor
    {
        public string CodArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public string Matricula { get; set; }
        public string CodProveedor { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime HoraEnvio { get; set; }
        public int idEnvio { get; set; }
        public int Estado { get; set; }
        public decimal? KilosEnviados { get; set; }
        public decimal? PesoNeto { get; set; }
        public bool? BasculaG { get; set; }
        public bool? BasculaP { get; set; }
        public string Observaciones { get; set; }
    }
}