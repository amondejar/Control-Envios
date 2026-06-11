using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlEnvios.Models
{
    public class ModeloProduccion
    {

        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha Inicio")]
        [Required]
        public string DtFechaInicio { get; set; }

        //[DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}")]
        //[DataType(DataType.Date, ErrorMessage = "Por favor, selecciona una fecha válida")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Fecha Fin")]
        [Required]
        public string DtFechaFin { get; set; }

        [Display(Name = "Proveedores")]
        public SelectList Proveedores { get; set; }  // Lista de Articulos.


        public string Proveedor { get; set; }


        public double TotalKilos { get; set; }

        public int Estado { get; set; }

        [Display(Name = "Neto")]
        public string PesoBascula { get; set; }
        [Display(Name = "Bascula Grande")]
        public bool? BasculaG { get; set; }
        [Display(Name = "Bascula Peque")]
        public bool? BasculaP { get; set; }
    }
}