using ControlEnvios.Models;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ControlEnvios.Controllers
{
    public class AccountController : Controller
    {
     


        // GET: Account

        public ActionResult Index()
        {
            return View();    
        }


        
     

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ModelProveedor prov)
        {
            
            if(!ModelState.IsValid)
            {
                return View(prov);
            }

            DateTime utcNow = DateTime.UtcNow;
            DateTime utcExpire = prov.Recuerdame ? utcNow.AddDays(5) : utcNow.AddMinutes(20);
            var authticket = new FormsAuthenticationTicket(2, prov.CodProveedor, utcNow, utcExpire,prov.Recuerdame,string.Empty,"/");

            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authticket));
            cookie.HttpOnly = true;
            if(prov.Recuerdame)
            {
                cookie.Expires = authticket.Expiration;
            }
            Response.Cookies.Add(cookie);


            using (BasculaEntities contexto = new BasculaEntities())
            {

                try
                {
                    var proveedor = contexto.PROVEEDOR.SingleOrDefault(p => p.CODPROVEEDOR == prov.CodProveedor && p.PASSWORD == prov.Password);
                    if (proveedor != null)
                    {
                        System.Web.HttpContext.Current.Session["Proveedor"] = proveedor;
                        System.Web.HttpContext.Current.Session["EsProveedor"] = true;
                        System.Web.HttpContext.Current.Session["EsGestor"] = false;
                        System.Web.HttpContext.Current.Session["EsProduccion"] = false;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        //Comprbamos is es un usuario para gestion
                        var usuario = contexto.USUARIOS.FirstOrDefault(a => a.ALIAS == prov.CodProveedor && a.PASSWORD == prov.Password && a.CHECKWEBADMIN == true && a.ID_PERFIL == 1);
                        if (usuario != null)
                        {
                            ModelModulo m = new ModelModulo();
                            m.EsGestor = true; // Es un gestor
                            System.Web.HttpContext.Current.Session["EsProveedor"] = false;
                            System.Web.HttpContext.Current.Session["EsGestor"] = true;
                            System.Web.HttpContext.Current.Session["EsProduccion"] = false;
                            return RedirectToAction("Index", "Home", m);
                        }
                        var usuarioProduccion = contexto.USUARIOS.FirstOrDefault(a => a.ALIAS == prov.CodProveedor && a.PASSWORD == prov.Password && a.CHECKWEBADMIN == true && a.ID_PERFIL == 5);
                        if(usuarioProduccion != null)
                        {
                            ModelModulo m = new ModelModulo();
                            m.EsProduccion = true; // Es producion
                            System.Web.HttpContext.Current.Session["EsProveedor"] = false;
                            System.Web.HttpContext.Current.Session["EsGestor"] = false;
                            System.Web.HttpContext.Current.Session["EsProduccion"] = true;
                            return RedirectToAction("Index", "Home", m);
                        }
                        else
                        {
                            TempData["Error"] = "Usario o contraseña incorrecta";
                            
                        }

                    }
                }
                catch(Exception exc)
                {
                    TempData["Error"] = exc.Message.ToString();
                    
                }
            }
            return View(prov);
        }
      

    }
}
