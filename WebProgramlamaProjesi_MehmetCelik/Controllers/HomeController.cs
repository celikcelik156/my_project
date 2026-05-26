using System;
using System.Linq;
using System.Web.Mvc;
using WebProgramlamaProjesi_MehmetCelik.Models;
using WebProgramlamaProjesi_MehmetCelik.Filters;

namespace WebProgramlamaProjesi_MehmetCelik.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Home
        public ActionResult Index()
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                ViewBag.Sliderlar = db.Sliderlar.Where(s => s.Aktif).OrderBy(s => s.Sira).ToList();
                ViewBag.Hizmetler = db.Hizmetler.Where(h => h.Aktif).OrderBy(h => h.Sira).Take(3).ToList();
                ViewBag.Portfoliolar = db.Portfoliolar.Where(p => p.Aktif).OrderBy(p => p.Sira).Take(6).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Home Index Error: " + ex.Message);
                ViewBag.Sliderlar = new System.Collections.Generic.List<Slider>();
                ViewBag.Hizmetler = new System.Collections.Generic.List<Hizmet>();
                ViewBag.Portfoliolar = new System.Collections.Generic.List<Portfolio>();
            }
            return View();
        }

        public ActionResult Services() 
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                ViewBag.Hizmetler = db.Hizmetler.Where(h => h.Aktif).OrderBy(h => h.Sira).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Services Error: " + ex.Message);
                ViewBag.Hizmetler = new System.Collections.Generic.List<Hizmet>();
            }
            return View();
        }

        [HttpGet]
        public ActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Contact(Iletisim iletisim)
        {
            if (ModelState.IsValid)
            {
                iletisim.OlusturmaTarihi = System.DateTime.Now;
                iletisim.Okundu = false;
                db.Iletisimler.Add(iletisim);
                db.SaveChanges();
                ViewBag.Mesaj = "Mesajınız başarıyla gönderildi!";
                return View();
            }
            return View(iletisim);
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
