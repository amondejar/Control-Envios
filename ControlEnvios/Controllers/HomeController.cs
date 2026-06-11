using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlEnvios.Models;
using System.Web.Security;
using ControlEnvios.Models;

namespace ControlEnvios.Controllers
{
    public class HomeController : Controller
    {
        

        [AllowAnonymous]
        public ActionResult Index(ModelModulo modulo = null)
        {
            List<MODULO> Modulos = new List<MODULO>();
            List<ModelModulo> ListaModuloIndex = new List<ModelModulo>();

            using (BasculaEntities contexto = new BasculaEntities())
            {


                try
                {
                    //Filtamos si hemos asignado un responsable o no para ver modulo de gestion
                    modulo.EsGestor = (bool)Session["EsGestor"];
                    modulo.EsProduccion = (bool)Session["EsProduccion"];

                    if (modulo.EsGestor && modulo.EsProduccion == false) //Cogemos modulo 6 y 3
                    {
                        Modulos = (from m in contexto.MODULO
                                   where m.IdModulo != 7 && m.IdModulo != 1
                                   select m).ToList();
                    }
                    else if (modulo.EsGestor == false && modulo.EsProduccion == false)  //Es proveedor 
                    {
                        Modulos = (from m in contexto.MODULO
                                   where m.IdModulo != 6 && m.IdModulo != 7
                                   select m).ToList();
                    }
                    else if (modulo.EsGestor == false && modulo.EsProduccion == true) // Produccion
                    {
                        Modulos = (from m in contexto.MODULO
                                   where m.IdModulo != 1 && m.IdModulo != 6
                                   select m).ToList();
                    }

                    foreach (var m in Modulos)
                    {
                        ListaModuloIndex.Add(new ModelModulo
                        {
                            IdModulo = m.IdModulo,
                            Nombre = m.NombreModulo,
                            Acction = m.Acction,
                            Controller = m.Controller,
                            Icono = m.icono,
                            bgname = m.bgName
                        });
                    }

                }
                catch(NullReferenceException)
                {
                    return Json(new { success = false, message = "Session caducada."}, JsonRequestBehavior.AllowGet);
                }
                catch(Exception exc)
                {
                    return Json(new { success = false, message = "Error: " + exc.Message }, JsonRequestBehavior.AllowGet);
                }
                

                

            }

            
            return View(ListaModuloIndex);
        }

       

      
       



    }
}