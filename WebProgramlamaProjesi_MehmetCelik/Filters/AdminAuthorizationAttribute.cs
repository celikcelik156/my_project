using System.Web;
using System.Web.Mvc;
using WebProgramlamaProjesi_MehmetCelik.Models;

namespace WebProgramlamaProjesi_MehmetCelik.Filters
{
    public class AdminAuthorizationAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!SessionHelper.IsLoggedIn())
            {
                return false;
            }

            return SessionHelper.IsAdmin();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Giris/Giris?message=YetkisizErisim");
        }
    }
}

