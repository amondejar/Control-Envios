using System.Web;
using System.Web.Optimization;

namespace ControlEnvios
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                         "~/Scripts/Common/Common.js",
                        "~/Scripts/Common/Resources.js",
                        "~/Scripts/CoreLibrary/framework-plugins.js",
                        "~/Scripts/Application/custom.js",
                        "~/Scripts/datatables.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Utilice la versión de desarrollo de Modernizr para desarrollar y obtener información. De este modo, estará
            // para la producción, use la herramienta de compilación disponible en https://modernizr.com para seleccionar solo las pruebas que necesite.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                       "~/Scripts/CoreLibrary/moment-with-locales.js",
                      "~/Scripts/CoreLibrary/bootstrap-datetimepicker.min.js",
                      "~/Scripts/CoreLibrary/toastr.min.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                       "~/Content/framework.css",
                          "~/Content/font-awesome.css",
                          "~/Content/menus.css",
                          "~/Content/bootstrap-datetimepicker.css",
                           "~/Content/toastr.css",
                           "~/Content/toastr.css"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/Scripts/CoreLibrary/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/Envio").Include(
                       "~/Scripts/Application/Envio.js"));

            bundles.Add(new ScriptBundle("~/bundles/Gestor").Include(
                      "~/Scripts/Application/Gestor.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/Produccion").Include(
                      "~/Scripts/Application/Produccion.js"));

            


            bundles.Add(new StyleBundle("~/Content/logincss").Include(
                     "~/Content/bootstrap.css",
                     "~/Content/font-awesome.css",
                      "~/Content/site.css"
                     ));


            bundles.Add(new StyleBundle("~/Content/toastcss").Include(
                   "~/Content/toast.css"));

          bundles.Add(new ScriptBundle("~/bundles/toast").Include(
                      "~/Scripts/Common/MessageError.js"));


           
        }
    }
}
