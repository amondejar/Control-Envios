using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlEnvios.Models
{
    public class ModelModulo
    {
        public int IdModulo { get; set; }
        public string Nombre { get; set; }
        public string Acction { get; set; }
        public string  Controller { get; set; }

        public string Icono { get; set; }

        public string bgname { get; set; }


        public bool EsGestor { get; set; }

        public bool EsProduccion { get; set; }
    }
}