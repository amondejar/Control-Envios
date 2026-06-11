using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControlEnvios.Models
{
    public class ModeloArticulo
    {
        public int CodArticulo { get; set; }
        public IList<ARTICULO> listaArticulo { get; set; }
        public IList<SelectListItem> ListaDeArticulos
        {

            get
            {
                var lista = (from item in listaArticulo
                             select new SelectListItem()
                             {
                                 Text = item.DESCRIPCION.ToString(CultureInfo.InvariantCulture),
                                 Value = item.CODARTICULO.ToString(CultureInfo.InvariantCulture)
                             }).ToList();
                return lista;
            }
            set { }

        }


        public ModeloArticulo()
        {

        }



    }
}