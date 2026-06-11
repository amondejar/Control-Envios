using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ControlEnvios.Models
{
    public class ModelProveedor
    {
        [Required(ErrorMessage = "Debe de ingresar su código de proveedor")]
        [StringLength(50)]
        [Display(Name ="Cod.Proveedor ")]
        public string CodProveedor  { get; set; }
        [Required(ErrorMessage ="Debe de ingresar su contraseña")]
        [StringLength(50)]
        [Display(Name ="Contraseña ")]
        public string Password { get; set; }


        [Display(Name = "Recuerdame")]
        public bool Recuerdame { get; set; }
        public string NombreProvedor { get; set; }
        
       


        public System.DateTime CreateDate { get; set; }

        public Nullable<System.DateTime> LastLoginDate { get; set; }

        
    }
}