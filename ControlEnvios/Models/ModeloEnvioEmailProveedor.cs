using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlEnvios.Models
{
    public class ModeloEnvioEmailProveedor
    {

        public int idEnvio { get; set; }
        public DateTime FechaHoraEnvio { get; set; }
        public string  Matricula { get; set; }
        public string NombreProveedor { get; set; }
        public string Email { get; set; }
    }
}