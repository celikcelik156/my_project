using System;
using System.Linq;
using System.Web.Mvc;
using WebProgramlamaProjesi_MehmetCelik.Models;

namespace WebProgramlamaProjesi_MehmetCelik.Controllers
{
    public class OtherPagesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Full Width Page
        public ActionResult FullWidth()
        {
            return View();
        }

        // GET: Sidebar Page
        public ActionResult Sidebar()
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                ViewBag.SonBloglar = db.Bloglar
                    .Where(b => b.Aktif)
                    .OrderByDescending(b => b.OlusturmaTarihi)
                    .Take(3)
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Sidebar Error: " + ex.Message);
                ViewBag.SonBloglar = new System.Collections.Generic.List<Blog>();
            }
            return View();
        }

        // GET: FAQ
        public ActionResult FAQ()
        {
            return View();
        }

        // GET: 404 Not Found
        public ActionResult NotFound404()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

