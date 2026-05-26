using System;
using System.Linq;
using System.Windows.Forms;
using Microsoft.EntityFrameworkCore;
using MarketAutomation.Data;
using MarketAutomation.Forms;
using MarketAutomation.Models;

namespace MarketAutomation
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Veritabanını oluştur ve test verilerini ekle
            SeedDatabase();

            Application.Run(new frmLogin());
        }

        static void SeedDatabase()
        {
            try
            {
                using (var db = new MarketDbContext())
                {
                    db.Database.EnsureCreated(); // Veritabanı yoksa oluştur

                    // Mevcut veritabanına sütunları hata fırlatmadan (zaten varsa) ekleme
                    try { db.Database.ExecuteSqlRaw("ALTER TABLE Products ADD COLUMN IsFavorite INTEGER NOT NULL DEFAULT 0;"); } catch { }
                    try { db.Database.ExecuteSqlRaw("ALTER TABLE Products ADD COLUMN ImagePath TEXT NULL;"); } catch { }

                    if (!db.Users.Any(u => u.Username == "kasiyer"))
                    {
                        db.Users.Add(new User 
                        { 
                            Username = "kasiyer", 
                            PasswordHash = Helpers.SecurityHelper.HashPassword("1234"), 
                            Role = "Kasiyer", 
                            FullName = "Örnek Kasiyer", 
                            IsActive = true 
                        });
                        db.SaveChanges();
                    }

                    var adminUser = db.Users.FirstOrDefault(u => u.Username == "admin");
                    if (adminUser != null)
                    {
                        adminUser.PasswordHash = Helpers.SecurityHelper.HashPassword("1234");
                        db.SaveChanges();
                    }

                    if (!db.Categories.Any())
                    {
                        db.Categories.Add(new Category { Name = "Gıda", Description = "Temel Gıda Ürünleri" });
                        db.Categories.Add(new Category { Name = "İçecek", Description = "Sıcak ve Soğuk İçecekler" });
                        db.Categories.Add(new Category { Name = "Manav", Description = "Sebze ve Meyve" });
                        db.SaveChanges();
                    }

                    if (!db.Suppliers.Any())
                    {
                        db.Suppliers.Add(new Supplier { CompanyName = "Yıldız Toptan Gıda", ContactName = "Ali Yıldız", Phone = "05551234567" });
                        db.SaveChanges();
                    }

                    if (!db.Customers.Any())
                    {
                        db.Customers.Add(new Customer { FullName = "Ahmet Yılmaz", Phone = "05321112233", DebtBalance = 150.50m });
                        db.SaveChanges();
                    }

                    if (!db.Products.Any())
                    {
                        db.Products.Add(new Product { Barcode = "8691234567890", Name = "Eti Burçak Bisküvi", PurchasePrice = 12m, SalePrice = 18m, StockQuantity = 100, CategoryId = 1, SupplierId = 1 });
                        db.Products.Add(new Product { Barcode = "8690000000001", Name = "Sütaş Ayran 1L", PurchasePrice = 20m, SalePrice = 28.5m, StockQuantity = 10, CategoryId = 2, SupplierId = 1 });
                        db.Products.Add(new Product { Barcode = "12345", Name = "Salkım Domates (KG)", PurchasePrice = 25m, SalePrice = 40m, StockQuantity = 250, CategoryId = 3, SupplierId = 1 });
                        db.SaveChanges();
                    }

                    if (!db.Campaigns.Any())
                    {
                        db.Campaigns.Add(new Campaign { CampaignName = "Burçak 3 Al 2 Öde", CampaignType = "3_AL_2_ODE", ProductId = 1, IsActive = true, StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddMonths(1) });
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veritabanı oluşturulurken hata: " + ex.Message, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}