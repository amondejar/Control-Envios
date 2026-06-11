using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlEnvios.Models
{
    public class ModeloEnvio
    {
        public int id { get; set; }

        [Display(Name ="Cod.Proveedor")]
        public string CodProveedor { get; set; }

        [Display(Name = "Nombre: ")]
        public string NomProveedor { get; set; }

        [Display(Name = "Fecha Envio")]
        public string FechaEnvio { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe de introlducir una hora de envío.")]
        [RegularExpression("^([01]?[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "Intoduzca una hora válida.")]
        [Display(Name = "Hora Envío")]
        public string HoraEnvio { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe de introducir un articulo.")]
        [Display(Name = "Artículo")]
        public SelectList TipoArtculos { get; set; }  // Lista de Articulos.


        public string Articulo { get; set; }

        public string NombreArticulo { get; set; }


        public bool SendEmail { get; set; }


        [Display(Name = "Kilos Enviados")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Debe de introducir los kilos que envía.")]
        public string KilosEnviados { get; set; }

        [Display(Name = "Estado")]
        public int EstadoEnvio { get; set; }

        [Display(Name = "Observaciones")]
        public string Observaciones { get; set; }

        [Display(Name = "Matrícula")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor introduzca la Matricula.")]
        [RegularExpression("^[A-Z0-9 a-z]*$",ErrorMessage="Por favor no introduzca guiones.")]
        public string Matricula { get; set; }

        public string Action { get; set; }

        public  string Envioid { get; set; }

        
        public string PesoNetoProveedor { get; set; } //Peso neto para el proveedor.

        [Display(Name = "Neto")]
        public string PesoBascula { get; set; }
        [Display(Name = "Báscula Grande")]
        public bool? BasculaG { get; set; }
        [Display(Name = "Báscula Pequeña")]
        public bool? BasculaP { get; set; }

        [Display(Name = "Hora Salida")]
        public string HoraSalida { get; set; }
    }  
    
}