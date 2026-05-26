using Microsoft.EntityFrameworkCore;
using MarketAutomation.Models;
using System;
using System.IO;
using System.Linq;

namespace MarketAutomation.Data
{
    public class MarketDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // LocalDB'deki "already exists" veya "cannot attach" hatalarını kökten çözmek 
                // ve uygulamanın her bilgisayarda sorunsuz çalışması için SQLite'a geçiş yapıyoruz.
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MarketDB.sqlite");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Seed Admin User
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Username = "admin",
                PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918", // SHA256 for 'admin'
                FullName = "System Administrator",
                Role = "Admin",
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1)
            });
        }
    }
}
