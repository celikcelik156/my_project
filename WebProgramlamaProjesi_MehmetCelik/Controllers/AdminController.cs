using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using WebProgramlamaProjesi_MehmetCelik.Filters;
using WebProgramlamaProjesi_MehmetCelik.Models;

namespace WebProgramlamaProjesi_MehmetCelik.Controllers
{
    [AdminAuthorization]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            ViewBag.ToplamBlog = db.Bloglar.Count();
            ViewBag.AktifBlog = db.Bloglar.Count(b => b.Aktif);
            ViewBag.ToplamSlider = db.Sliderlar.Count();
            ViewBag.ToplamPortfolio = db.Portfoliolar.Count();
            ViewBag.ToplamHizmet = db.Hizmetler.Count();
            ViewBag.OkunmamisMesaj = db.Iletisimler.Count(i => !i.Okundu);
            ViewBag.ToplamMesaj = db.Iletisimler.Count();
            ViewBag.ToplamKullanici = db.Kullanicilar.Count();
            
            return View();
        }

        // Get: BLOG YÖNETİMİ
        public ActionResult BlogListesi()
        {
            var bloglar = db.Bloglar.OrderByDescending(b => b.OlusturmaTarihi).ToList();
            return View(bloglar);
        }

        public ActionResult BlogEkle()
        {
            ViewBag.KategoriId = new SelectList(db.Kategoriler, "Id", "Ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BlogEkle(Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.OlusturmaTarihi = DateTime.Now;
                blog.Aktif = true;
                blog.GoruntulenmeSayisi = 0;

                if (blog.ResimDosyasi != null && blog.ResimDosyasi.ContentLength > 0)
                {
                    string dosyaAdi = Path.GetFileName(blog.ResimDosyasi.FileName);

                    string klasorYolu = Server.MapPath("~/algocak/img/");

                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    string tamYol = Path.Combine(klasorYolu, dosyaAdi);
                    blog.ResimDosyasi.SaveAs(tamYol);

                    blog.ResimYolu = "/algocak/img/" + dosyaAdi;
                }

                db.Bloglar.Add(blog);
                db.SaveChanges();
                return RedirectToAction("BlogListesi");
            }

            ViewBag.KategoriId = new SelectList(db.Kategoriler, "Id", "Ad", blog.KategoriId);
            return View(blog);
        }
        public ActionResult BlogDuzenle(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Blog blog = db.Bloglar.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.KategoriId = new SelectList(db.Kategoriler, "Id", "Ad", blog.KategoriId);
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BlogDuzenle(Blog blog)
        {
            if (ModelState.IsValid)
            {
                blog.GuncellemeTarihi = DateTime.Now;
                if (blog.ResimDosyasi != null && blog.ResimDosyasi.ContentLength > 0)
                {
                    string dosyaAdi = Path.GetFileName(blog.ResimDosyasi.FileName);

                    string klasorYolu = Server.MapPath("~/algocak/img/");

                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    string tamYol = Path.Combine(klasorYolu, dosyaAdi);

                    blog.ResimDosyasi.SaveAs(tamYol);

                    blog.ResimYolu = "/algocak/img/" + dosyaAdi;
                }

                db.Entry(blog).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Blog post updated successfully!";
                return RedirectToAction("BlogListesi");
            }
            ViewBag.KategoriId = new SelectList(db.Kategoriler, "Id", "Ad", blog.KategoriId);
            return View(blog);
        }

        public ActionResult BlogSil(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Blog blog = db.Bloglar.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        [HttpPost, ActionName("BlogSil")]
        [ValidateAntiForgeryToken]
        public ActionResult BlogSilConfirmed(int id)
        {
            Blog blog = db.Bloglar.Find(id);
            db.Bloglar.Remove(blog);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Blog post deleted successfully!";
            return RedirectToAction("BlogListesi");
        }

        // GET: SLIDER YÖNETİMİ
        public ActionResult SliderListesi()
        {
            var sliderlar = db.Sliderlar.OrderBy(s => s.Sira).ToList();
            return View(sliderlar);
        }

        public ActionResult SliderEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SliderEkle(Slider slider)
        {
            ModelState.Remove("ResimYolu");

            if (ModelState.IsValid)
            {
                slider.Aktif = true;
                
                if (slider.Sira == 0)
                {
                    var maxSira = db.Sliderlar.Any() ? db.Sliderlar.Max(s => s.Sira) : 0;
                    slider.Sira = maxSira + 1;
                }

                if (slider.ResimDosyasi != null && slider.ResimDosyasi.ContentLength > 0)
                {
                    string dosyaAdi = Path.GetFileName(slider.ResimDosyasi.FileName);

                    string klasorYolu = Server.MapPath("~/algocak/img/");

                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    string tamYol = Path.Combine(klasorYolu, dosyaAdi);
                    slider.ResimDosyasi.SaveAs(tamYol);

                    slider.ResimYolu = "/algocak/img/" + dosyaAdi;
                }

                db.Sliderlar.Add(slider);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Slider added successfully!";
                return RedirectToAction("SliderListesi");
            }
            return View(slider);
        }

        public ActionResult SliderDuzenle(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Slider slider = db.Sliderlar.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SliderDuzenle(Slider slider)
        {
            if (ModelState.IsValid)
            {
                if (slider.ResimDosyasi != null && slider.ResimDosyasi.ContentLength > 0)
                {
                    string dosyaAdi = Path.GetFileName(slider.ResimDosyasi.FileName);
                    string klasorYolu = Server.MapPath("~/algocak/img/");

                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    string tamYol = Path.Combine(klasorYolu, dosyaAdi);
                    slider.ResimDosyasi.SaveAs(tamYol);

                    slider.ResimYolu = "/algocak/img/" + dosyaAdi;
                }

                db.Entry(slider).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Slider updated successfully!";
                return RedirectToAction("SliderListesi");
            }
            return View(slider);
        }
        public ActionResult SliderSil(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Slider slider = db.Sliderlar.Find(id);
            if (slider == null)
            {
                return HttpNotFound();
            }
            return View(slider);
        }

        [HttpPost, ActionName("SliderSil")]
        [ValidateAntiForgeryToken]
        public ActionResult SliderSilConfirmed(int id)
        {
            Slider slider = db.Sliderlar.Find(id);
            db.Sliderlar.Remove(slider);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Slider deleted successfully!";
            return RedirectToAction("SliderListesi");
        }

        // GET: PORTFOLIO YÖNETİMİ 
        public ActionResult PortfolioListesi()
        {
            var portfoliolar = db.Portfoliolar.OrderBy(p => p.Sira).ToList();
            return View(portfoliolar);
        }

        public ActionResult PortfolioEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PortfolioEkle(Portfolio portfolio)
        {
            ModelState.Remove("ResimYolu");

            if (ModelState.IsValid)
            {
                portfolio.Aktif = true;
                if (portfolio.Sira == 0)
                {
                    var maxSira = db.Portfoliolar.Any() ? db.Portfoliolar.Max(p => p.Sira) : 0;
                    portfolio.Sira = maxSira + 1;
                }
                if (portfolio.ResimDosyasi != null && portfolio.ResimDosyasi.ContentLength > 0)
                {
                    string dosyaAdi = Path.GetFileName(portfolio.ResimDosyasi.FileName);

                    string klasorYolu = Server.MapPath("~/algocak/img/");

                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    string tamYol = Path.Combine(klasorYolu, dosyaAdi);
                    portfolio.ResimDosyasi.SaveAs(tamYol);

                    portfolio.ResimYolu = "/algocak/img/" + dosyaAdi;
                }

                db.Portfoliolar.Add(portfolio);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Portfolio item added successfully!";
                return RedirectToAction("PortfolioListesi");
            }
            return View(portfolio);
        }
        public ActionResult PortfolioDuzenle(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Portfolio portfolio = db.Portfoliolar.Find(id);
            if (portfolio == null)
            {
                return HttpNotFound();
            }
            return View(portfolio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PortfolioDuzenle(Portfolio portfolio)
        {
            if (ModelState.IsValid)
            {
                if (portfolio.ResimDosyasi != null && portfolio.ResimDosyasi.ContentLength > 0)
                {
                    string dosyaAdi = Path.GetFileName(portfolio.ResimDosyasi.FileName);
                    string klasorYolu = Server.MapPath("~/algocak/img/");

                    if (!Directory.Exists(klasorYolu))
                    {
                        Directory.CreateDirectory(klasorYolu);
                    }

                    string tamYol = Path.Combine(klasorYolu, dosyaAdi);
                    portfolio.ResimDosyasi.SaveAs(tamYol);

                    portfolio.ResimYolu = "/algocak/img/" + dosyaAdi;
                }

                db.Entry(portfolio).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Portfolio item updated successfully!";
                return RedirectToAction("PortfolioListesi");
            }
            return View(portfolio);
        }
        public ActionResult PortfolioSil(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Portfolio portfolio = db.Portfoliolar.Find(id);
            if (portfolio == null)
            {
                return HttpNotFound();
            }
            return View(portfolio);
        }

        [HttpPost, ActionName("PortfolioSil")]
        [ValidateAntiForgeryToken]
        public ActionResult PortfolioSilConfirmed(int id)
        {
            Portfolio portfolio = db.Portfoliolar.Find(id);
            db.Portfoliolar.Remove(portfolio);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Portfolio item deleted successfully!";
            return RedirectToAction("PortfolioListesi");
        }

        // GET: KATEGORİ YÖNETİMİ
        public ActionResult KategoriListesi()
        {
            var kategoriler = db.Kategoriler.ToList();
            return View(kategoriler);
        }

        public ActionResult KategoriEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriEkle(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                db.Kategoriler.Add(kategori);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Category added successfully!";
                return RedirectToAction("KategoriListesi");
            }
            return View(kategori);
        }

        public ActionResult KategoriDuzenle(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Kategori kategori = db.Kategoriler.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriDuzenle(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kategori).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Category updated successfully!";
                return RedirectToAction("KategoriListesi");
            }
            return View(kategori);
        }

        public ActionResult KategoriSil(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Kategori kategori = db.Kategoriler.Find(id);
            if (kategori == null)
            {
                return HttpNotFound();
            }
            return View(kategori);
        }

        [ValidateAntiForgeryToken]
        public ActionResult KategoriSilConfirmed(int id)
        {
            Kategori kategori = db.Kategoriler.Find(id);
            db.Kategoriler.Remove(kategori);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Category deleted successfully!";
            return RedirectToAction("KategoriListesi");
        }

        // GET: İLETİŞİM YÖNETİMİ
        public ActionResult IletisimListesi()
        {
            var iletisimler = db.Iletisimler.OrderByDescending(i => i.OlusturmaTarihi).ToList();
            return View(iletisimler);
        }

        public ActionResult IletisimDetay(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Iletisim iletisim = db.Iletisimler.Find(id);
            if (iletisim == null)
            {
                return HttpNotFound();
            }
            iletisim.Okundu = true;
            db.SaveChanges();
            return View(iletisim);
        }
        public ActionResult IletisimSil(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Iletisim iletisim = db.Iletisimler.Find(id);
            if (iletisim == null)
            {
                return HttpNotFound();
            }
            return View(iletisim);
        }

        [HttpPost, ActionName("IletisimSil")]
        [ValidateAntiForgeryToken]
        public ActionResult IletisimSilConfirmed(int id)
        {
            Iletisim iletisim = db.Iletisimler.Find(id);
            db.Iletisimler.Remove(iletisim);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Message deleted successfully!";
            return RedirectToAction("IletisimListesi");
        }

        // GET: HİZMET YÖNETİMİ
        public ActionResult HizmetListesi()
        {
            var hizmetler = db.Hizmetler.OrderBy(h => h.Sira).ToList();
            return View(hizmetler);
        }

        public ActionResult HizmetEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HizmetEkle(Hizmet hizmet)
        {
            if (ModelState.IsValid)
            {
                hizmet.Aktif = true;
                if (hizmet.Sira == 0)
                {
                    var maxSira = db.Hizmetler.Any() ? db.Hizmetler.Max(h => h.Sira) : 0;
                    hizmet.Sira = maxSira + 1;
                }
                db.Hizmetler.Add(hizmet);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Service added successfully!";
                return RedirectToAction("HizmetListesi");
            }
            return View(hizmet);
        }

        public ActionResult HizmetDuzenle(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Hizmet hizmet = db.Hizmetler.Find(id);
            if (hizmet == null)
            {
                return HttpNotFound();
            }
            return View(hizmet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HizmetDuzenle(Hizmet hizmet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hizmet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Service updated successfully!";
                return RedirectToAction("HizmetListesi");
            }
            return View(hizmet);
        }

        public ActionResult HizmetSil(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Hizmet hizmet = db.Hizmetler.Find(id);
            if (hizmet == null)
            {
                return HttpNotFound();
            }
            return View(hizmet);
        }

        [HttpPost, ActionName("HizmetSil")]
        [ValidateAntiForgeryToken]
        public ActionResult HizmetSilConfirmed(int id)
        {
            Hizmet hizmet = db.Hizmetler.Find(id);
            db.Hizmetler.Remove(hizmet);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Service deleted successfully!";
            return RedirectToAction("HizmetListesi");
        }

        // GET: KULLANICI YÖNETİMİ
        public ActionResult KullaniciListesi()
        {
            var kullanicilar = db.Kullanicilar.OrderBy(k => k.KullaniciAdi).ToList();
            return View(kullanicilar);
        }

        public ActionResult KullaniciDuzenle(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Kullanici kullanici = db.Kullanicilar.Find(id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }
            return View(kullanici);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KullaniciDuzenle(Kullanici kullanici)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kullanici).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "User updated successfully!";
                return RedirectToAction("KullaniciListesi");
            }
            return View(kullanici);
        }

        public ActionResult KullaniciAktifPasif(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }
            Kullanici kullanici = db.Kullanicilar.Find(id);
            if (kullanici == null)
            {
                return HttpNotFound();
            }
            kullanici.Aktif = !kullanici.Aktif;
            db.SaveChanges();
            TempData["SuccessMessage"] = $"User {(kullanici.Aktif ? "activated" : "deactivated")} successfully!";
            return RedirectToAction("KullaniciListesi");
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
