using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebProgramlamaProjesi_MehmetCelik.Filters;
using WebProgramlamaProjesi_MehmetCelik.Models;

namespace WebProgramlamaProjesi_MehmetCelik.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Blog
        public ActionResult Index(string search)
        {
            try
            {
                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                var bloglar = db.Bloglar.Where(b => b.Aktif);

                if (!string.IsNullOrEmpty(search))
                {
                    bloglar = bloglar.Where(b => b.Baslik.Contains(search) || b.Icerik.Contains(search));
                }

                bloglar = bloglar.OrderByDescending(b => b.OlusturmaTarihi);

                ViewBag.SonBloglar = db.Bloglar
                    .Where(b => b.Aktif)
                    .OrderByDescending(b => b.OlusturmaTarihi)
                    .Take(3)
                    .ToList();
                ViewBag.Arama = search;

                return View(bloglar.ToList());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Blog Index Error: " + ex.Message);
                return View(new System.Collections.Generic.List<Blog>());
            }
        }

        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    ViewBag.ErrorMessage = "Blog post ID is required.";
                    return View((Blog)null);
                }

                Blog blog = db.Bloglar
                    .FirstOrDefault(b => b.Id == id && b.Aktif);

                if (blog == null)
                {
                    ViewBag.ErrorMessage = "The blog post you are looking for does not exist or is not available.";
                    ViewBag.SonBloglar = db.Bloglar
                        .Where(b => b.Aktif)
                        .OrderByDescending(b => b.OlusturmaTarihi)
                        .Take(3)
                        .ToList();
                    return View((Blog)null);
                }

                blog.GoruntulenmeSayisi++;
                db.SaveChanges();

                ViewBag.SonBloglar = db.Bloglar
                    .Where(b => b.Aktif && b.Id != id)
                    .OrderByDescending(b => b.OlusturmaTarihi)
                    .Take(3)
                    .ToList();

                return View(blog);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Blog Details Error: " + ex.Message);
                ViewBag.ErrorMessage = "An error occurred while loading the blog post.";
                ViewBag.SonBloglar = db.Bloglar
                    .Where(b => b.Aktif)
                    .OrderByDescending(b => b.OlusturmaTarihi)
                    .Take(3)
                    .ToList();
                return View((Blog)null);
            }
        }

        //veritabanını serbest bırakma
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
