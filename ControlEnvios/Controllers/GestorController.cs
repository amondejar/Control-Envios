using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControlEnvios.Models;
using PagedList;
using System.Data.SqlClient;
using SmptEmailSendProveedor;

namespace ControlEnvios.Controllers
{


    public class GestorController : Controller
    {
        //Subclase de Gestor 
        public class EnviosProveedor
        {

            public string NombreArticulo { get; set; }
            public string Matricula { get; set; }
            public string CodProveedor { get; set; }
            public string NomProveedor { get; set; }
            public string FechaEnvio { get; set; }
            public string HoraEnvio { get; set; }
            public string HoraSalida { get; set; }
            public int id { get; set; }
            public int Estado { get; set; }
            public decimal? KilosEnviados { get; set; }
            public decimal? PesoBascula { get; set; }
            public bool? BasculaG { get; set; }
            public bool? BasculaP { get; set; }
            public string Observaciones { get; set; }

        }


        ModelGestor model = new ModelGestor();


        public static List<ModeloEnvio> ListaModeloEnvioPorFechas = new List<ModeloEnvio>();

        // GET: Gestor
        public ActionResult Index(int? page)
        {
            Session["CurrentPage"] = 1;
            ViewBag.ControlLista = "0";
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            ModelGestor mgestor = new ModelGestor();
            mgestor = PrecargaEnvio();
            return PartialView("~/Views/Gestor/_IntroDatos.cshtml", mgestor);
        }
        public GestorController () {}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ModelGestor mgestor)
        { 
            //Cargamos en sesion los datos enviados
            Session["FechaIniGestor"] = mgestor.DtFechaInicio;
            Session["FechaFinGEstor"] = mgestor.DtFechaFin;
            Session["CodProveedorGestor"] = mgestor.Proveedor;
            Session["CodArticuloGestor"] = mgestor.Articulo;
            Session["mgestor"] = mgestor;
            ViewBag.ControlLista = "1";
            return View("Index");
        }

        public ActionResult RecuperaFiltroEnvios(string valor1, string valor2)
        {
            if (Session["mgestor"] != null)
            {
                ListaModeloEnvioPorFechas = ListaEnviosFiltradosPorfechas(Session["mgestor"] as ModelGestor);
            }
            if (ListaModeloEnvioPorFechas != null && ListaModeloEnvioPorFechas.Count > 0)
            {
                return PartialView("~/Views/Gestor/_DestallesEnviosFechas.cshtml", ListaModeloEnvioPorFechas);
            }
            return PartialView("~/Views/Gestor/_DestallesEnviosFechas.cshtml", new List<ModeloEnvio>());
        }


        [HttpGet]
        public ActionResult NavegaConsultaEnvios()
        {
            ModelGestor mgestor = new ModelGestor();
            mgestor = PrecargaEnvio();
            return PartialView("~/Views/Gestor/_IntroDatos.cshtml",mgestor);
        }

        public ModelGestor PrecargaEnvio()
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

                //Parte de Carga combo articulos
                List<SelectListItem> Articulos = contexto.ARTICULO.Where(a => a.ACTIVAWEB != false).OrderBy(n => n.ID_ARTICULO)
                    .Select(n => new SelectListItem
                    {
                        Text = n.DESCRIPCION.ToString(),
                        Value = n.CODARTICULO.ToString()

                    }).ToList();

                var Articulo = new SelectListItem()
                {
                    Value = null,
                    Text = "---Selecciona un Articulo---"
                };
                Articulos.Insert(0, Articulo);
                model.Articulos = new SelectList(Articulos, "Value", "Text");
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
                if(mg.Proveedor == null && mg.Articulo == null)
                {
                    var _ResultadoProcEnvios = contexto.Database.SqlQuery<EnviosProveedor>("LISTAENVIOSFECHAPROVEEDOR @fechaEnvio,@fechafin,@codproveedor,@codArticulo",
                                                new SqlParameter("@fechaEnvio", DateEnvio),
                                                new SqlParameter("@fechafin",DateFin),
                                                new SqlParameter("codproveedor",""),
                                                new SqlParameter("codArticulo","")).ToList();
                    //Ordenar por fecha de envio
                    //var resultado_ord = _ResultadoProcEnvios.OrderBy(a => a.FechaEnvio);


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
                             HoraSalida = env.HoraSalida,
                             
                             Observaciones = env.Observaciones                      
                        };

                        ListaEnvios.Add(ModEnv);

                    }
                    _ResultadoProcEnvios = null; // Problema de reenumeracion en el FOREACH, por eso lo ponemos a null
                    
                }
                else if(mg.Proveedor != null && mg.Articulo == null)
                {
                    var _ResultadoProcEnvios = contexto.Database.SqlQuery<EnviosProveedor>("LISTAENVIOSFECHAPROVEEDOR @fechaEnvio,@fechafin,@codproveedor,@codArticulo",
                                                new SqlParameter("@fechaEnvio", DateEnvio),
                                                new SqlParameter("@fechafin", DateFin),
                                                new SqlParameter("codproveedor", mg.Proveedor),
                                                new SqlParameter("codArticulo","")).ToList();

                    foreach (var env in _ResultadoProcEnvios)
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
                            HoraSalida = env.HoraSalida,
                            Observaciones = string.IsNullOrEmpty(env.Observaciones) == true ? "NO HAY OBSERVACIONES" : env.Observaciones



                        };

                        ListaEnvios.Add(ModEnv);

                    }
                    _ResultadoProcEnvios = null;
                }
                else if(mg.Proveedor != null && mg.Articulo != null)
                {
                    var _ResultadoProcEnvios = contexto.Database.SqlQuery<EnviosProveedor>("LISTAENVIOSFECHAPROVEEDOR @fechaEnvio,@fechafin,@codproveedor,@codArticulo",
                                               new SqlParameter("@fechaEnvio", DateEnvio),
                                               new SqlParameter("@fechafin", DateFin),
                                               new SqlParameter("codproveedor", mg.Proveedor),
                                               new SqlParameter("codArticulo", mg.Articulo)).ToList();

                    foreach (var env in _ResultadoProcEnvios)
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
                            HoraSalida = env.HoraSalida,
                            Observaciones = string.IsNullOrEmpty(env.Observaciones) == true ? "NO HAY OBSERVACIONES" : env.Observaciones



                        };

                        ListaEnvios.Add(ModEnv);

                    }
                    _ResultadoProcEnvios = null;

                }
                else if(mg.Proveedor == null && mg.Articulo != null)
                {
                    var _ResultadoProcEnvios = contexto.Database.SqlQuery<EnviosProveedor>("LISTAENVIOSFECHAPROVEEDOR @fechaEnvio,@fechafin,@codproveedor,@codArticulo",
                                               new SqlParameter("@fechaEnvio", DateEnvio),
                                               new SqlParameter("@fechafin", DateFin),
                                               new SqlParameter("codproveedor", ""),
                                               new SqlParameter("codArticulo", mg.Articulo)).ToList();

                    foreach (var env in _ResultadoProcEnvios)
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
                            HoraSalida = env.HoraSalida,
                            Observaciones = string.IsNullOrEmpty(env.Observaciones) == true ? "NO HAY OBSERVACIONES" : env.Observaciones



                        };

                        ListaEnvios.Add(ModEnv);

                    }
                    _ResultadoProcEnvios = null;

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
                        new SqlParameter("@idEnvio",id)).ToList();

                    if (resultado != null)
                    {
                        //Ordenar por fecha de envio
                         var resultado_ord = resultado.OrderBy(a => a.FechaEnvio);

                        foreach(var env in resultado_ord)
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
                                envio.HoraSalida = env.HoraSalida;
                                envio.Observaciones = string.IsNullOrEmpty(env.Observaciones) ? "NO HAY OBSERVACIONES" : env.Observaciones;
                        }

                        return PartialView("~/Views/Gestor/_DetalleGestor.cshtml", envio);
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
        [HttpGet]
        public ActionResult CancelSendEnvio(string id)
        {
         
            try
            {
                using (BasculaEntities contexto = new Models.BasculaEntities())
                {

                    List<ModeloEnvio> ListaResultado = new List<ModeloEnvio>();
                   

                    //contexto.Database.SqlQuery<ModeloEnvio>("CANCELAENVIO @Id",
                    //    new SqlParameter("@id", id));

                    int valor = contexto.CANCELAENVIO(int.Parse(id));

                    //recogemos los envios actuales despues de actualizar el envios a tipo 4 o cancelado
                    var resultado = contexto.Database.SqlQuery<EnviosProveedor>("TODOSLOSENVIOSPROVEE").ToList();

                    if (resultado != null)
                    {
                        var resultadoFiltrado = resultado.Where(r => Convert.ToDateTime(r.FechaEnvio) >= Convert.ToDateTime(Session["FechaIniGestor"] as string)
                            && Convert.ToDateTime(r.FechaEnvio) <= Convert.ToDateTime(Session["FechaFinGestor"] as string));
                        if (!string.IsNullOrEmpty(Session["CodProveedorGestor"] as string))
                            resultadoFiltrado = resultadoFiltrado.Where(f => f.CodProveedor == Session["CodProveedorGestor"] as string);

                        //Enviamos el correo electronico al proveedor para que informarle de su cancelacion del envio.
                        //Cogemos una copia para realizar el envio de email al proveedor
                        var datosProveedorSendMailCancel = contexto.Database.SqlQuery<ModeloEnvioEmailProveedor>("CANCELAENVIOEMAIL @id", new SqlParameter("@id", int.Parse(id))).SingleOrDefault();
                        //Comprobamos que el proveedor tenga correo electronico.
                        if(!string.IsNullOrEmpty(datosProveedorSendMailCancel.Email.Trim()))
                        {
                            string Error = string.Empty;
                            SendEmail email = new SendEmail(datosProveedorSendMailCancel.Email, "Cancelación de envío matricula: "+ datosProveedorSendMailCancel.Matricula , "Estimado proveedor le comunicamos que su envio referente a la matricula indicada en el asunto de este correo ha sido cancelado.");
                            email.PrepararEnvioMail();
                            if(!email.EnviarEmail(ref Error))
                            {
                             //Tratar el error externamente.   
                            }

                        }
                        
                        foreach(var envioFiltrado in resultadoFiltrado)
                        {
                            ModeloEnvio envio = new ModeloEnvio()
                            {
                                 CodProveedor = envioFiltrado.CodProveedor,
                                 NomProveedor = envioFiltrado.NomProveedor,
                                 Matricula = envioFiltrado.Matricula,
                                 NombreArticulo = envioFiltrado.NombreArticulo,
                                 FechaEnvio = envioFiltrado.FechaEnvio.ToString(),
                                 HoraEnvio = envioFiltrado.HoraEnvio.ToString(),
                                 KilosEnviados = envioFiltrado.KilosEnviados.ToString(),
                                 PesoBascula = envioFiltrado.PesoBascula.ToString(),
                                 BasculaG = envioFiltrado.BasculaG,
                                 BasculaP = envioFiltrado.BasculaP,
                                 EstadoEnvio = envioFiltrado.Estado,
                                 Observaciones = string.IsNullOrEmpty(envioFiltrado.Observaciones) ? "NO HAY OBSERVACIONES" : envioFiltrado.Observaciones
                                  
   
                                 
                            };

                            ListaResultado.Add(envio);
                        }

                        
                        return PartialView("~/Views/Gestor/_DestallesEnviosFechas.cshtml", ListaResultado);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error al cargar los Envios." }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch(Exception exc)
            {

                return Json(new { success = false, message = "Error: " + exc.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private bool EnviaEmailProveedor(string EmailProveedor,string Asunto,string CuerpoMensaje)
        {
            bool resultado = true;


                
                
            return resultado;
        }

     }

 }
