using System;
using System.Linq;
using System.Web.Mvc;
using WebProgramlamaProjesi_MehmetCelik.Models;

namespace WebProgramlamaProjesi_MehmetCelik.Controllers
{
    public class PortfolioController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Portfolio - 1 Column
        public ActionResult Index()
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                if (!db.Portfoliolar.Any())
                {
                    SeedPortfolios();
                }

                var portfoliolar = db.Portfoliolar
                    .Where(p => p.Aktif)
                    .OrderBy(p => p.Sira)
                    .ToList();

                return View(portfoliolar);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Portfolio Index Error: " + ex.Message);
                return View(new System.Collections.Generic.List<Portfolio>());
            }
        }

        // GET: Portfolio - 2 Column
        public ActionResult TwoColumn()
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                var portfoliolar = db.Portfoliolar
                    .Where(p => p.Aktif)
                    .OrderBy(p => p.Sira)
                    .ToList();

                return View(portfoliolar);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Portfolio TwoColumn Error: " + ex.Message);
                return View(new System.Collections.Generic.List<Portfolio>());
            }
        }

        // GET: Portfolio - 3 Column
        public ActionResult ThreeColumn()
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                var portfoliolar = db.Portfoliolar
                    .Where(p => p.Aktif)
                    .OrderBy(p => p.Sira)
                    .ToList();

                return View(portfoliolar);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Portfolio ThreeColumn Error: " + ex.Message);
                return View(new System.Collections.Generic.List<Portfolio>());
            }
        }

        // GET: Portfolio Details
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    ViewBag.ErrorMessage = "Portfolio item ID is required.";
                    return View((Portfolio)null);
                }

                Portfolio portfolio = db.Portfoliolar
                    .FirstOrDefault(p => p.Id == id && p.Aktif);

                if (portfolio == null)
                {
                    ViewBag.ErrorMessage = "The portfolio item you are looking for does not exist or is not available.";
                    ViewBag.RelatedPortfolios = db.Portfoliolar
                        .Where(p => p.Aktif)
                        .OrderBy(p => p.Sira)
                        .Take(4)
                        .ToList();
                    return View((Portfolio)null);
                }

                ViewBag.RelatedPortfolios = db.Portfoliolar
                    .Where(p => p.Aktif && p.Id != id)
                    .OrderBy(p => p.Sira)
                    .Take(4)
                    .ToList();

                return View(portfolio);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Portfolio Details Error: " + ex.Message);
                ViewBag.ErrorMessage = "An error occurred while loading the portfolio item.";
                ViewBag.RelatedPortfolios = db.Portfoliolar
                    .Where(p => p.Aktif)
                    .OrderBy(p => p.Sira)
                    .Take(4)
                    .ToList();
                return View((Portfolio)null);
            }
        }

        // Portföy projelerini başlat (seed data) - Internal method
        private void SeedPortfolios()
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                if (!db.Portfoliolar.Any())
                {
                    var portfoliolar = new System.Collections.Generic.List<Portfolio>
                    {
                        new Portfolio
                        {
                            Baslik = "Algocak Proje 1",
                            Aciklama = "Modern ve şık tasarım ile web geliştirme projeleri",
                            ResimYolu = "/algocak/img/portfolio_1.png",
                            Link = "",
                            Sira = 1,
                            Aktif = true
                        },
                        new Portfolio
                        {
                            Baslik = "Algocak Proje 2",
                            Aciklama = "Responsive ve kullanıcı dostu arayüz tasarımları",
                            ResimYolu = "/algocak/img/portfolio_2.png",
                            Link = "",
                            Sira = 2,
                            Aktif = true
                        },
                        new Portfolio
                        {
                            Baslik = "Algocak Proje 3",
                            Aciklama = "İnovatif çözümler ve yaratıcı tasarım yaklaşımları",
                            ResimYolu = "/algocak/img/portfolio_3.png",
                            Link = "",
                            Sira = 3,
                            Aktif = true
                        },
                        new Portfolio
                        {
                            Baslik = "Algocak Proje 4",
                            Aciklama = "Profesyonel web uygulamaları ve mobil çözümler",
                            ResimYolu = "/algocak/img/portfolio_4.png",
                            Link = "",
                            Sira = 4,
                            Aktif = true
                        },
                        new Portfolio
                        {
                            Baslik = "Algocak Proje 5",
                            Aciklama = "E-ticaret ve kurumsal web siteleri geliştirme",
                            ResimYolu = "/algocak/img/portfolio_5.png",
                            Link = "",
                            Sira = 5,
                            Aktif = true
                        },
                        new Portfolio
                        {
                            Baslik = "Algocak Proje 6",
                            Aciklama = "SEO uyumlu ve performans odaklı web çözümleri",
                            ResimYolu = "/algocak/img/portfolio_6.png",
                            Link = "",
                            Sira = 6,
                            Aktif = true
                        }
                    };

                    foreach (var portfolio in portfoliolar)
                    {
                        db.Portfoliolar.Add(portfolio);
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Seed Portfolios Error: " + ex.Message);
            }
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

