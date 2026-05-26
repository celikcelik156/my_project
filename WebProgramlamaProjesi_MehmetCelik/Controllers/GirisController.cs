using System;
using System.Linq;
using System.Web.Mvc;
using WebProgramlamaProjesi_MehmetCelik.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebProgramlamaProjesi_MehmetCelik.Controllers
{
    public class GirisController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Giris
        public ActionResult Giris()
        {
            if (SessionHelper.IsLoggedIn())
            {
                if (SessionHelper.IsAdmin())
                    return RedirectToAction("Index", "Admin");
                else
                    return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== LOGIN METHOD CALLED ===");
                System.Diagnostics.Debug.WriteLine($"Email: {email}");

                if (!db.Database.Exists())
                {
                    System.Diagnostics.Debug.WriteLine("Database does not exist. Creating...");
                    db.Database.Create();
                    System.Diagnostics.Debug.WriteLine("Database created.");
                }

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.ErrorMessage = "E-posta ve şifre zorunludur.";
                    return View("Giris");
                }

                string hashedPassword = HashPassword(password);
                System.Diagnostics.Debug.WriteLine($"Password hashed. Looking for user with email: {email}");

                var kullanici = db.Kullanicilar.FirstOrDefault(k => k.Email == email && k.Aktif);

                if (kullanici == null)
                {
                    System.Diagnostics.Debug.WriteLine("User not found or inactive.");
                    ViewBag.ErrorMessage = "E-posta veya şifre hatalı.";
                    return View("Giris");
                }

                System.Diagnostics.Debug.WriteLine($"User found: {kullanici.Email}, Admin: {kullanici.Admin}");

                if (kullanici.Sifre == hashedPassword)
                {
                    SessionHelper.SetUserSession(
                        kullanici.Id,
                        kullanici.Email,
                        kullanici.AdSoyad ?? kullanici.KullaniciAdi,
                        kullanici.Admin
                    );

                    System.Diagnostics.Debug.WriteLine("Login successful. Redirecting...");
                    if (kullanici.Admin)
                        return RedirectToAction("Index", "Admin");
                    else
                        return RedirectToAction("Index", "Home");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Password mismatch.");
                    ViewBag.ErrorMessage = "E-posta veya şifre hatalı.";
                    return View("Giris");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Login Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ViewBag.ErrorMessage = "Giriş sırasında bir hata oluştu: " + ex.Message;
                return View("Giris");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string username, string email, string password, string adSoyad)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("=== REGISTER METHOD CALLED ===");
                System.Diagnostics.Debug.WriteLine($"Username: {username}, Email: {email}");

                if (!db.Database.Exists())
                {
                    System.Diagnostics.Debug.WriteLine("Database does not exist. Creating...");
                    db.Database.Create();
                    System.Diagnostics.Debug.WriteLine("Database created.");
                }

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.ErrorMessage = "Lütfen tüm zorunlu alanları doldurun.";
                    return View("Giris");
                }

                var existingUser = db.Kullanicilar.FirstOrDefault(k => k.Email == email || k.KullaniciAdi == username);
                if (existingUser != null)
                {
                    ViewBag.ErrorMessage = "Bu e-posta veya kullanıcı adı zaten kullanılıyor.";
                    return View("Giris");
                }

                var yeniKullanici = new Kullanici
                {
                    KullaniciAdi = username,
                    Email = email,
                    Sifre = HashPassword(password),
                    AdSoyad = adSoyad,
                    Admin = false,
                    Aktif = false
                };

                System.Diagnostics.Debug.WriteLine("Adding user to database...");
                db.Kullanicilar.Add(yeniKullanici);
                int result = db.SaveChanges();
                System.Diagnostics.Debug.WriteLine($"SaveChanges result: {result}, UserId: {yeniKullanici.Id}");

                if (result > 0 && yeniKullanici.Id > 0)
                {
                    System.Diagnostics.Debug.WriteLine("Registration successful. Redirecting to Home...");
                    ViewBag.ErrorMessage = "Kayıt başarılı. Hesabınız admin onayına gönderildi; onaylandıktan sonra giriş yapabilirsiniz.";
                }
                else
                {
                    ViewBag.ErrorMessage = "Kayıt başarısız oldu. Lütfen tekrar deneyin.";
                }

                return RedirectToAction("Giris");
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                string errorMsg = "Validation errors: ";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMsg += validationError.PropertyName + ": " + validationError.ErrorMessage + "; ";
                    }
                }
                System.Diagnostics.Debug.WriteLine("Register Validation Error: " + errorMsg);
                ViewBag.ErrorMessage = "Kayıt sırasında doğrulama hatası: " + errorMsg;
                return View("Giris");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Register Error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                ViewBag.ErrorMessage = "Kayıt sırasında bir hata oluştu: " + ex.Message;
                return View("Giris");
            }
        }

        public ActionResult Logout()
        {
            SessionHelper.ClearSession();
            return RedirectToAction("Giris");
        }

        // GET: Şifremi Unuttum
        public ActionResult SifremiUnuttum()
        {
            return View();
        }

        // POST: Şifremi Unuttum
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SifremiUnuttum(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    ViewBag.ErrorMessage = "Lütfen e-posta adresinizi girin.";
                    return View();
                }

                if (!db.Database.Exists())
                {
                    db.Database.Create();
                }

                var kullanici = db.Kullanicilar.FirstOrDefault(k => k.Email == email && k.Aktif);

                if (kullanici == null)
                {
                    ViewBag.SuccessMessage = "Eğer bu e-posta adresi sistemde kayıtlıysa, şifre sıfırlama bağlantısı gönderilmiştir.";
                    return View();
                }

                ViewBag.SuccessMessage = "Şifre sıfırlama bağlantısı e-posta adresinize gönderilmiştir. Lütfen e-posta kutunuzu kontrol edin.";
                
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SifremiUnuttum Error: " + ex.Message);
                ViewBag.ErrorMessage = "Bir hata oluştu. Lütfen tekrar deneyin.";
                return View();
            }
        }

        // Şifre Hashleme Metodu
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