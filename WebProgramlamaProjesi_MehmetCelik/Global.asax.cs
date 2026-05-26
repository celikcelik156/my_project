using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using WebProgramlamaProjesi_MehmetCelik.Models;

namespace WebProgramlamaProjesi_MehmetCelik
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Database.SetInitializer(new DbInitializer());
        }
    }
}