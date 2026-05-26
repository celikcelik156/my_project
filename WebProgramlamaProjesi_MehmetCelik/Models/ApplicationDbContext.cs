using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Blog> Bloglar { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Slider> Sliderlar { get; set; }
        public DbSet<Portfolio> Portfoliolar { get; set; }
        public DbSet<Iletisim> Iletisimler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
    }

    public class DbInitializer : CreateDatabaseIfNotExists<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            if (!context.Kullanicilar.Any(k => k.Email == "admin@admin.com"))
            {
                var admin = new Kullanici
                {
                    KullaniciAdi = "admin",
                    Email = "admin@admin.com",
                    Sifre = HashPassword("admin123"),
                    AdSoyad = "Sistem Yöneticisi",
                    Admin = true,
                    Aktif = true
                };

                context.Kullanicilar.Add(admin);
                context.SaveChanges();
            }

            if (!context.Portfoliolar.Any())
            {
                var portfoliolar = new List<Portfolio>
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
                    context.Portfoliolar.Add(portfolio);
                }
                context.SaveChanges();
            }

            base.Seed(context);
        }

        // Şifre Hashleme Yardımcı Metodu
        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}