using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlEnvios.Models;
using PagedList;
using System.Data.SqlClient;

namespace ControlEnvios.Controllers
{
    public class ProduccionController : Controller
    {

        //Subclase de Produccion
        public class EnviosProveedor
        {

            public string NombreArticulo { get; set; }
            public string Matricula { get; set; }
            public string CodProveedor { get; set; }
            public string NomProveedor { get; set; }
            public string FechaEnvio { get; set; }
            public string HoraEnvio { get; set; }
            public int id { get; set; }
            public int Estado { get; set; }
            public decimal? KilosEnviados { get; set; }
            public decimal? PesoBascula { get; set; }
            public bool? BasculaG { get; set; }
            public bool? BasculaP { get; set; }
            public string Observaciones { get; set; }

        }


        public static List<ModeloEnvio> ListaModeloEnvioPorFechas = new List<ModeloEnvio>();

        ModeloProduccion model = new ModeloProduccion();

        // GET: Gestor
        public ActionResult Index(int? page)
        {
            Session["CurrentPage"] = 1;
            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            ModeloProduccion mgestor = new ModeloProduccion();
            mgestor = PrecargaEnvio();
            return PartialView("~/Views/Produccion/_IntroDatos.cshtml", mgestor);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelGestor mgestor)
        {

            ListaModeloEnvioPorFechas = ListaEnviosFiltradosPorfechas(mgestor);

            return RedirectToAction("Index");
        }

        public ActionResult RecuperaFiltroEnvios(string valor1, string valor2)
        {
            if (ListaModeloEnvioPorFechas != null && ListaModeloEnvioPorFechas.Count > 0)
            {
                return PartialView("~/Views/Produccion/_DestallesEnviosFechas.cshtml", ListaModeloEnvioPorFechas);
            }
            return PartialView("~/Views/Produccion/_DestallesEnviosFechas.cshtml", new List<ModeloEnvio>());
        }


        public ModeloProduccion PrecargaEnvio()
        {

            using (BasculaEntities contexto = new BasculaEntities())
            {
                List<SelectListItem> Proveedores = contexto.PROVEEDOR.AsNoTracking().Where(a => a.CODPOSTAL != "z").OrderBy(n => n.CODPROVEEDOR)
                     .Select(n => new SelectListItem
                     {
                         Text = n.NOMBRE.ToString(),
                         Value = n.CODPROVEEDOR.ToString()

                     }).ToList();

                var Proveedor = new SelectListItem()
                {
                    Value = null,
                    Text = "---Selecciona un Proveedor---"
                };
                Proveedores.Insert(0, Proveedor);
                model.Proveedores = new SelectList(Proveedores, "Value", "Text");
            }

            return model;
        }

        public List<ModeloEnvio> ListaEnviosFiltradosPorfechas(ModelGestor mg)
        {
            DateTime DateEnvio = Convert.ToDateTime(mg.DtFechaInicio);
            DateTime DateFin = Convert.ToDateTime(mg.DtFechaFin);

            List<ModeloEnvio> ListaEnvios = new List<ModeloEnvio>();
            

            using (BasculaEntities contexto = new BasculaEntities())
            {            
                if(mg.Proveedor == null)
                {
                    var _ResultadoProcEnvios = contexto.Database.SqlQuery<EnviosProveedor>("LISTAENVIOSFECHAPROVEEDOR @fechaEnvio,@fechafin,@codproveedor",
                                                new SqlParameter("@fechaEnvio", DateEnvio),
                                                new SqlParameter("@fechafin", DateFin),
                                                new SqlParameter("codproveedor", "")).ToList();

                    foreach(var env in _ResultadoProcEnvios)
                    {
                        ModeloEnvio ModEnv = new ModeloEnvio()
                        {
                             NombreArticulo = env.NombreArticulo,
                             Matricula = env.Matricula,
                             CodProveedor = env.CodProveedor,
                             NomProveedor = env.NomProveedor,
                             FechaEnvio = env.FechaEnvio,
                             HoraEnvio = env.HoraEnvio,
                             id = env.id,
                             KilosEnviados = env.KilosEnviados.ToString(),
                             PesoBascula = env.PesoBascula.ToString(),
                             BasculaG = env.BasculaG,
                             BasculaP = env.BasculaP,
                             EstadoEnvio = env.Estado,
                             Observaciones = env.Observaciones
                        };

                        ListaEnvios.Add(ModEnv);

                    }
                    _ResultadoProcEnvios = null; // Problema de reenumeracion en el FOREACH, por eso lo ponemos a null
                    
                }
                
                    return ListaEnvios;
               }

                return null;
            }

        [HttpGet]
        public ActionResult Detail(string id)
        {

            ModeloEnvio envio = new ModeloEnvio();
            try
            {
                using (BasculaEntities contexto = new Models.BasculaEntities())
                {

                    var resultado = contexto.Database.SqlQuery<EnviosProveedor>("LISTAENVIOSIDPROVEEDOR @IdEnvio",
                        new SqlParameter("@idEnvio", id)).ToList();

                    if (resultado != null)
                    {

                        foreach (var env in resultado)
                        {
                            envio.NombreArticulo = env.NombreArticulo;
                            envio.Matricula = env.Matricula;
                            envio.CodProveedor = env.CodProveedor;
                            envio.NomProveedor = env.NomProveedor;
                            envio.FechaEnvio = env.FechaEnvio;
                            envio.HoraEnvio = env.HoraEnvio;
                            envio.id = env.id;
                            envio.KilosEnviados = env.KilosEnviados.ToString();
                            envio.PesoBascula = env.PesoBascula.ToString();
                            envio.BasculaG = env.BasculaG;
                            envio.BasculaP = env.BasculaP;
                            envio.EstadoEnvio = env.Estado;
                            envio.Observaciones = env.Observaciones;
                        }

                        return PartialView("~/Views/Produccion/_DetalleProduccion.cshtml", envio);
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
        }
    }
