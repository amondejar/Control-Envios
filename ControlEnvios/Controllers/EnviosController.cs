using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlEnvios.Models;
using ControlEnvios.AuxClass;
using System.Globalization;
using ControlEnvios.Models;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;

namespace ControlEnvios.Controllers
{
    public class EnviosController : Controller
    {

        public class EnviosProveedor
        {

            public string CodArticulo { get; set; }
            public string Descripcion { get; set; }
            public string Matricula { get; set; }
            public string CodProveedor { get; set; }
            public DateTime FechaEnvio { get; set; }
            public DateTime HoraEnvio { get; set; }
            public int idEnvio { get; set; }
            public int Estado { get; set; }
            public decimal? KilosEnviados { get; set; }
            public decimal? PesadaNeto { get; set; }
            public string Observaciones { get; set; }

        }



        ModeloEnvio model = new ModeloEnvio();
        ModelProveedor proveedor = new ModelProveedor();

        public ActionResult Index()
        {
            Session["CurrentPage"] = 1;
            return View();
        }




        [HttpGet]
        public ActionResult Create()
        {
            ModeloEnvio envio = new Models.ModeloEnvio();
            envio = PrecargaEnvio();
            envio.Action = "Create";
            envio.FechaEnvio = DateTime.Now.ToShortDateString();
            envio.HoraEnvio = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00");
            return PartialView("~/Views/Envios/_CrearEnvios.cshtml", envio);
        }


        public ActionResult RecuperarEnvios(string Valor1, string Valor2)
        {
            //Devolvemos los envios realizados desde el dia actual en adelante

            List<ModeloEnvio> EnviosProveedor = new List<ModeloEnvio>();

            using (BasculaEntities contexto = new BasculaEntities())
            {
                model.CodProveedor = (System.Web.HttpContext.Current.Session["Proveedor"] as PROVEEDOR).CODPROVEEDOR.ToString();
                DateTime fechaActual = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                //Recojo todos los envios realizados a partir de hoy hacia adelante y Estado 0(Enviado)
                //var lista = contexto.ENVIOMERCANCIA.Where(a => a.FECHAENVIO >= fechaActual && a.ESTADO == 0);

                //List<PROC_ENVIOPESADA_Result> lista = contexto.PROC_ENVIOPESADA(fechaActual).ToList();
                var EnvioPesada = contexto.Database.SqlQuery<EnviosProveedor>("ENVIOPESADA @fechahoy,@Proveedor", new SqlParameter("@fechaHoy", fechaActual),new SqlParameter("@Proveedor",model.CodProveedor)).ToList();
                if(EnvioPesada != null)
                {
                    foreach (var elemento in EnvioPesada)
                    {
                        ModeloEnvio envio = new ModeloEnvio()
                        {
                            Articulo = elemento.CodArticulo,

                            NombreArticulo = elemento.Descripcion,
                            Matricula = (string.IsNullOrEmpty(elemento.Matricula) == true) ? "" : elemento.Matricula,
                            CodProveedor = elemento.CodProveedor,
                            FechaEnvio = elemento.FechaEnvio.ToShortDateString(),
                            HoraEnvio = (elemento.HoraEnvio == null) ? "" : elemento.HoraEnvio.Hour.ToString("00") + ":" + elemento.HoraEnvio.Minute.ToString("00"),
                            id = elemento.idEnvio,
                            KilosEnviados = Convert.ToString(elemento.KilosEnviados),
                            EstadoEnvio = elemento.Estado,
                            PesoNetoProveedor = (elemento.PesadaNeto == null) ? "0" : elemento.PesadaNeto.ToString(),
                            Observaciones = elemento.Observaciones



                        };

                        EnviosProveedor.Add(envio);
                    }


                }
               
            }

            return PartialView("~/Views/Envios/_DetalleEnvios.cshtml", EnviosProveedor);
           
        }

        [HttpGet]
        public ActionResult Detail(string id)
        {

            ModeloEnvio env = new Models.ModeloEnvio();

            try
            {
                using (BasculaEntities contexto = new Models.BasculaEntities())
                {

                    var EnvioDetalle = contexto.Database.SqlQuery<ModelDetalleProveedor>("LISTAENVIOSIDPROVEEDOR @id", new SqlParameter("@id", int.Parse(id))).ToList();            
                    if (EnvioDetalle != null)
                    {
                        foreach(var DetalleEnvio in EnvioDetalle)
                        {

                            ModeloEnvio envio = new Models.ModeloEnvio()
                            {
                                Articulo = DetalleEnvio.CodArticulo,

                                NombreArticulo = DetalleEnvio.NombreArticulo,  //no funciona revisar no devuelve la descripcion del articulo.
                                Matricula = (string.IsNullOrEmpty(DetalleEnvio.Matricula) == true) ? "" : DetalleEnvio.Matricula,
                                CodProveedor = DetalleEnvio.CodProveedor,
                                FechaEnvio = DetalleEnvio.FechaEnvio.ToShortDateString(),
                                HoraEnvio = (DetalleEnvio.HoraEnvio == null) ? "" : DetalleEnvio.HoraEnvio.Hour.ToString("00") + ":" + DetalleEnvio.HoraEnvio.Minute.ToString("00"),
                                id = DetalleEnvio.idEnvio,
                                KilosEnviados = Convert.ToString(DetalleEnvio.KilosEnviados),
                                EstadoEnvio = DetalleEnvio.Estado,
                                PesoNetoProveedor = (DetalleEnvio.PesoNeto == null) ? "0" : DetalleEnvio.PesoNeto.ToString(),
                                Observaciones = DetalleEnvio.Observaciones
                                
                            };
                            env = envio;
                            
                        }
                        return PartialView("~/Views/Envios/_DetalleEnvio.cshtml", env);
                        
                    }
                    else
                    {
                        return Json(new { success = false, message = "No existe el envio." }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception exc)
            {
                return Json(new { success = false, message = "Error: " + exc.Message }, JsonRequestBehavior.AllowGet);
            }
        }



        // GET: Envios
        public ModeloEnvio PrecargaEnvio()
        {

            using (BasculaEntities contexto = new BasculaEntities())
            {
                List<SelectListItem> Articulos = contexto.ARTICULO.AsNoTracking().Where(a => a.ACTIVAWEB == true).OrderBy(n => n.CODARTICULO)
                    .Select(n => new SelectListItem
                    {
                        Text = n.DESCRIPCION.ToString(),
                        Value = n.CODARTICULO.ToString()

                    }).ToList();

                var art = new SelectListItem()
                {
                    Value = null,
                    Text = "---Selecciona un Articulo---"
                };
                Articulos.Insert(0, art);
                ViewBag.TiposArticulos = new SelectList(Articulos, "Value", "Text");
            }
            model.CodProveedor = (System.Web.HttpContext.Current.Session["Proveedor"] as PROVEEDOR).CODPROVEEDOR.ToString();
            model.NomProveedor = (System.Web.HttpContext.Current.Session["Proveedor"] as PROVEEDOR).NOMBRE.ToString();
            model.Observaciones = "";
            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModeloEnvio model)
        {
            List<ModeloEnvio> listaModelosCreados = new List<Models.ModeloEnvio>();
            CUPOPROVEEDOR cupoProv = new CUPOPROVEEDOR();
            decimal? totalPorcentage = 0;
            decimal? porcentageCupoProveedor = 0;
            decimal? totalKilosEnviadosSemana = 0;
            decimal? _kilosPrimerEnvio = 0;

            //Comprobar si esta limitado.
            cupoProv = ProvedorTieneCupoActivo();

            if (cupoProv.ACTIVACUPO == true && cupoProv != null)
            {

                //Dame kilos del cupo semanal
                decimal KilosCupoSemana = DameKilosCupoSemana();
                bool EsPrimerEnvio = false;

                //recogemos el porcentage asignado a ese proveedor con cupo activo
                porcentageCupoProveedor = cupoProv.PORCENTAGEASIG;

                //recogemos los kilos enviados en la semana actual

                totalKilosEnviadosSemana = KilosEnviadosPorveedorSemana(model.CodProveedor, ref EsPrimerEnvio);

                if (EsPrimerEnvio)
                {
                    _kilosPrimerEnvio = Convert.ToDecimal(model.KilosEnviados);

                }




                //Calculamos el porcentage que lleva y realizamos el calculo para saber si se ha superado.

                if (!CalculaPorcentageAsignado(totalKilosEnviadosSemana, KilosCupoSemana, porcentageCupoProveedor, EsPrimerEnvio, Convert.ToDecimal(model.KilosEnviados)))
                {
                    TempData["success"] = "Cupo excedido,por favor contacte con el responsable de compras.";
                    return RedirectToAction("Index");
                }



                using (BasculaEntities contexto = new BasculaEntities())
                {


                    try
                    {
                        ENVIOMERCANCIA env = new ENVIOMERCANCIA()
                        {
                            CODPROVEEDOR = model.CodProveedor,
                            CODARTICULO = model.Articulo,
                            MATRICULA = model.Matricula,
                            ESTADO = 1, //Enviado
                            FECHAENVIO = Convert.ToDateTime(model.FechaEnvio),
                            HORAENVIO = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + model.HoraEnvio.ToString()),
                            KILOSENVIADOS = Convert.ToInt32(model.KilosEnviados),
                            OBSERVACIONES = (model.Observaciones == null) ? "" : model.Observaciones.ToString()

                        };

                        contexto.ENVIOMERCANCIA.Add(env);
                        contexto.SaveChanges();
                        DateTime fechaActual = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                        //Recojo todos los envios realizados a partir de hoy hacia adelante.
                        var lista = contexto.ENVIOMERCANCIA.Where(a => a.FECHAENVIO >= fechaActual);
                        foreach (var l in lista)
                        {
                            ModeloEnvio envio = new ModeloEnvio()
                            {
                                Articulo = l.CODARTICULO,
                                NombreArticulo = contexto.ARTICULO.FirstOrDefault(a => a.CODARTICULO == l.CODARTICULO).DESCRIPCION.ToString(),
                                Matricula = l.MATRICULA,
                                CodProveedor = l.CODPROVEEDOR,
                                FechaEnvio = l.FECHAENVIO.ToShortDateString(),
                                HoraEnvio = (l.HORAENVIO == null) ? "" : l.HORAENVIO.ToString(),
                                id = l.IDENVIO,
                                KilosEnviados = Convert.ToString(l.KILOSENVIADOS)

                            };
                            listaModelosCreados.Add(envio);
                        }
                        TempData["success"] = Resources.Envios.EnvInsertado;

                    }
                    catch (Exception exc)
                    {
                        TempData["success"] = exc.Message;
                        return RedirectToAction("Index");

                    }


                }
            }
            else
            {

                using (BasculaEntities contexto = new BasculaEntities())
                {


                    try
                    {
                        ENVIOMERCANCIA env = new ENVIOMERCANCIA()
                        {
                            CODPROVEEDOR = model.CodProveedor,
                            CODARTICULO = model.Articulo,
                            MATRICULA = model.Matricula,
                            ESTADO = 1, //Enviado
                            FECHAENVIO = Convert.ToDateTime(model.FechaEnvio),
                            HORAENVIO = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " " + model.HoraEnvio.ToString()),
                            KILOSENVIADOS = Convert.ToInt32(model.KilosEnviados),
                            OBSERVACIONES = (model.Observaciones == null) ? "" : model.Observaciones.ToString()

                        };

                        contexto.ENVIOMERCANCIA.Add(env);
                        contexto.SaveChanges();
                        DateTime fechaActual = Convert.ToDateTime(DateTime.Now.ToShortDateString());

                        //Recojo todos los envios realizados a partir de hoy hacia adelante.
                        var lista = contexto.ENVIOMERCANCIA.Where(a => a.FECHAENVIO >= fechaActual);
                        foreach (var l in lista)
                        {
                            ModeloEnvio envio = new ModeloEnvio()
                            {
                                Articulo = l.CODARTICULO,
                                NombreArticulo = contexto.ARTICULO.FirstOrDefault(a => a.CODARTICULO == l.CODARTICULO).DESCRIPCION.ToString(),
                                Matricula = l.MATRICULA,
                                CodProveedor = l.CODPROVEEDOR,
                                FechaEnvio = l.FECHAENVIO.ToShortDateString(),
                                HoraEnvio = (l.HORAENVIO == null) ? "" : l.HORAENVIO.ToString(),
                                id = l.IDENVIO,
                                KilosEnviados = Convert.ToString(l.KILOSENVIADOS)

                            };
                            listaModelosCreados.Add(envio);
                        }
                        TempData["success"] = Resources.Envios.EnvInsertado;

                    }
                    catch (Exception exc)
                    {
                        TempData["success"] = exc.Message;
                        return RedirectToAction("Index");

                    }


                }



            }
            return RedirectToAction("Index");

        }

        private bool CalculaPorcentageAsignado(decimal? kilosEnviados, decimal? kilosCupo, decimal? porcentageAsignado, bool EsPrimerEnvio, decimal? _KilosPeticionProveedor)
        {
            bool resultado = false;
            decimal? KilosAsignadosEnCupo = 0;
            decimal? SumaKilosAsignadosMasEnviados = 0;
            if (!EsPrimerEnvio)
            {

                KilosAsignadosEnCupo = (kilosCupo * porcentageAsignado) / 100; //Kilos asignados segun cupo
                SumaKilosAsignadosMasEnviados = kilosEnviados + _KilosPeticionProveedor;
                resultado = (SumaKilosAsignadosMasEnviados <= KilosAsignadosEnCupo);
            }
            else
            {
                resultado = true;
            }
            return resultado;
        }


        private string ObtenerPrimerdiaSemana()
        {
            string _fInicio;
            _fInicio = DateTime.Now.AddDays(-1 * ((int)DateTime.Now.DayOfWeek - 1)).ToShortDateString();
            return _fInicio;
        }
        private string ObtenerUltimodiaSemana()
        {
            string _fFin;
            _fFin = DateTime.Now.AddDays(-1 * ((int)DateTime.Now.DayOfWeek - 7)).ToShortDateString();
            return _fFin;
        }
        //Retorna los el total de los kilos enviados 

        //Seguir por aqui--- tema fechas....
        private decimal? KilosEnviadosPorveedorSemana(string CodProveedor, ref bool EsPrimerEnvio)
        {

            decimal? resultado = 0;
            decimal? totalKilos = 0;
            EsPrimerEnvio = true;

            using (BasculaEntities ctx = new BasculaEntities())
            {
                var _totalKilosProv = ctx.KILOS_SEMANA_PROVEEDOR(Convert.ToDateTime(ObtenerPrimerdiaSemana()), Convert.ToDateTime(ObtenerUltimodiaSemana()),
                                                                    CodProveedor);

                foreach (var t in _totalKilosProv)
                {
                    totalKilos = t.Value;
                }

                if (totalKilos > 0)
                {
                    resultado = totalKilos;
                    EsPrimerEnvio = false;
                }
                return resultado;
            }

        }

        //Comprueba que si el porveedor tiene cupo activo
        private CUPOPROVEEDOR ProvedorTieneCupoActivo()
        {
            CUPOPROVEEDOR cupoProveedor = new CUPOPROVEEDOR();
            string _codProveedorSession = (System.Web.HttpContext.Current.Session["Proveedor"] as PROVEEDOR).CODPROVEEDOR;

            using (BasculaEntities ctx = new BasculaEntities())
            {
                cupoProveedor = ctx.CUPOPROVEEDOR.SingleOrDefault(p => p.CODPROVEE == _codProveedorSession);
                if (cupoProveedor != null && cupoProveedor.ACTIVACUPO == true)
                {
                    return cupoProveedor;

                }
                return cupoProveedor;
            }
        }

        //Devfuelve los kilos del cupo semanal.
        private decimal DameKilosCupoSemana()
        {
            decimal resultado = 0;
            using (BasculaEntities ctx = new BasculaEntities())
            {
                resultado = ctx.CUPOKG.Single(k => k.IDCUPOKG == 1).CUPOKILOS;
            }
            return resultado;
        }


        [HttpGet]
        public ActionResult Edit(string id)
        {

            ModeloEnvio envio = new Models.ModeloEnvio();

            using (BasculaEntities contexto = new Models.BasculaEntities())
            {
                var ConsultaEnvio = contexto.ENVIOMERCANCIA.FirstOrDefault(a => a.IDENVIO.ToString() == id);
                if (ConsultaEnvio != null)
                {

                    envio.Action = "Edit";
                    envio.id = ConsultaEnvio.IDENVIO;
                    envio.CodProveedor = ConsultaEnvio.CODPROVEEDOR;
                    envio.NomProveedor = contexto.PROVEEDOR.FirstOrDefault(a => a.CODPROVEEDOR == ConsultaEnvio.CODPROVEEDOR).NOMBRE;
                    envio.FechaEnvio = ConsultaEnvio.FECHAENVIO.ToShortDateString();
                    envio.KilosEnviados = Convert.ToString(Math.Truncate(ConsultaEnvio.KILOSENVIADOS));
                    envio.Matricula = ConsultaEnvio.MATRICULA;
                    envio.HoraEnvio = ConsultaEnvio.HORAENVIO.Value.Hour.ToString("00") + ":" + ConsultaEnvio.HORAENVIO.Value.Minute.ToString("00");
                    envio.Observaciones = ConsultaEnvio.OBSERVACIONES;
                    ViewBag.TiposArticulos = new SelectList(this.ListaArticulos(), "Value", "Text", ConsultaEnvio.CODARTICULO);

                }

                return PartialView("~/Views/Envios/_CrearEnvios.cshtml", envio);
            }
        }


        private List<SelectListItem> ListaArticulos()
        {
            List<SelectListItem> Articulos = new List<SelectListItem>();
            using (BasculaEntities contexto = new Models.BasculaEntities())
            {
                Articulos = contexto.ARTICULO.AsNoTracking().Where(a => a.ACTIVAWEB == true).OrderBy(n => n.CODARTICULO)
                                                   .Select(n => new SelectListItem
                                                   {
                                                       Text = n.DESCRIPCION.ToString(),
                                                       Value = n.CODARTICULO.ToString()

                                                   }).ToList();

            }
            return Articulos;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ModeloEnvio UpdateEnvio)
        {


            try
            {

                using (BasculaEntities contexto = new Models.BasculaEntities())
                {

                    var EnvioParaCambiar = contexto.ENVIOMERCANCIA.FirstOrDefault(a => a.IDENVIO == UpdateEnvio.id);

                    if (EnvioParaCambiar != null)
                    {
                        EnvioParaCambiar.IDENVIO = UpdateEnvio.id;
                        EnvioParaCambiar.CODARTICULO = UpdateEnvio.Articulo;
                        EnvioParaCambiar.CODPROVEEDOR = UpdateEnvio.CodProveedor;
                        EnvioParaCambiar.FECHAENVIO = Convert.ToDateTime(UpdateEnvio.FechaEnvio);
                        EnvioParaCambiar.HORAENVIO = Convert.ToDateTime(UpdateEnvio.HoraEnvio);
                        EnvioParaCambiar.KILOSENVIADOS = Convert.ToDecimal(UpdateEnvio.KilosEnviados.ToString());
                        EnvioParaCambiar.MATRICULA = UpdateEnvio.Matricula;
                        EnvioParaCambiar.OBSERVACIONES = string.IsNullOrEmpty(UpdateEnvio.Observaciones) == true ? "" : UpdateEnvio.Observaciones;

                        //Guardamos los cambios del envio
                        contexto.ENVIOMERCANCIA.Attach(EnvioParaCambiar);
                        contexto.Entry(EnvioParaCambiar).State = System.Data.Entity.EntityState.Modified;
                        contexto.SaveChanges();
                        TempData["success"] = "Cambios realizados correctamente.";
                    }
                    else
                    {
                        TempData["success"] = "Error al actualizar el envio seleccionado.";
                    }
                }

            }
            catch (Exception exc)
            {
                TempData["success"] = "Error: " + exc.Message;
            }


            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult Delete(string id)
        {
            try
            {

                using (BasculaEntities contexto = new Models.BasculaEntities())
                {

                    //Recuperamos el objeto a eliminar
                    var ElminaEnvio = contexto.ENVIOMERCANCIA.FirstOrDefault(a => a.IDENVIO.ToString() == id);
                    if (ElminaEnvio != null)
                    {
                        contexto.ENVIOMERCANCIA.Remove(ElminaEnvio);
                        contexto.SaveChanges();

                        TempData["success"] = "Envio eliminado correctamente.";
                    }
                    else
                    {
                        TempData["error"] = "Error al elieminar el envio.";
                    }


                }
            }
            catch (Exception exc)
            {

                TempData["error"] = "Error: " + exc.Message;

            }
            return RedirectToAction("Index");
        }

       



        public static int GetIso8601WeekOfYear(DateTime time)
        {
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

        public static DateTime FirstDateOfWeek(int year, int weekOfYear, System.Globalization.CultureInfo ci)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = (int)ci.DateTimeFormat.FirstDayOfWeek - (int)jan1.DayOfWeek;
            DateTime firstWeekDay = jan1.AddDays(daysOffset);
            int firstWeek = ci.Calendar.GetWeekOfYear(jan1, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
            if ((firstWeek <= 1 || firstWeek >= 52) && daysOffset >= -3)
            {
                weekOfYear -= 1;
            }
            return firstWeekDay.AddDays(weekOfYear * 7);
        }
        [HttpGet]
        public JsonResult CargaEnvios()
        {
            BasculaEntities edb = new BasculaEntities(); 
            // UsersContext u = new UsersContext();  

            var jsonData = new
            {
                total = 1,
                page = 1,
                records = edb.ENVIOMERCANCIA.ToList().Count,
                rows = (
                  from emp in edb.ENVIOMERCANCIA.ToList()
                  select new
                  {
                      idEnvio = emp.IDENVIO,
                      cell = new string[] {
                       emp.CODPROVEEDOR.ToString()
                    }
                  }).ToArray()
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }

    }
}