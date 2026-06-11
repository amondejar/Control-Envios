using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ControlEnvios
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.Session["Proveedor"] = null;
            System.Web.HttpContext.Current.Session["EsProveedor"] = false;
            System.Web.HttpContext.Current.Session["ESGestor"] = false;
            System.Web.HttpContext.Current.Session["FechaIniGestor"] = "";
            System.Web.HttpContext.Current.Session["FechaFinGestor"] = "";
            System.Web.HttpContext.Current.Session["CodProveedorGestor"] = "";
            System.Web.HttpContext.Current.Session["CodArticuloGestor"] = "";
            System.Web.HttpContext.Current.Session["mgestor"] = null;
           
        }
        
    }
}
