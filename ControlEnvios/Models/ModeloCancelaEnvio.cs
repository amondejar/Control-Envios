using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlEnvios.Models
{
    public class ModeloCancelaEnvio
    {
        public int id { get; set; }
        public string CodProveedor { get; set; }
        public string NomProveedor { get; set; }
        public DateTime FechaEnvio { get; set; }
        public DateTime HoraEnvio { get; set; }
        public SelectList TipoArtculos { get; set; }  // Lista de Articulos.
        public string Articulo { get; set; }
        public string NombreArticulo { get; set; }
        public bool SendEmail { get; set; }
        public decimal KilosEnviados { get; set; }
        public int EstadoEnvio { get; set; }
        public string Observaciones { get; set; }
        public string Matricula { get; set; }
        public string Action { get; set; }
        public string Envioid { get; set; }
        public decimal PesoNetoProveedor { get; set; } //Peso neto para el proveedor.
        public string PesoBascula { get; set; }
        public bool? BasculaG { get; set; }
        public bool? BasculaP { get; set; }
    }
}