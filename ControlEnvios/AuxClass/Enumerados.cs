using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ControlEnvios.AuxClass
{
    public class Enumerados
    {

        public enum Estado
        {
            Nuevo,
            Modificado,
            Cancelado
        }

        public enum EstadoEnvio
        {
            Enviado,
            EnBascula,
            Pesado,
            Cancelado,

        }


        public enum EstadoLimitado
        {
              EstaLimitado,
              NoEstaLimitado,
              NoEstaActivo
        }
    }

     
}