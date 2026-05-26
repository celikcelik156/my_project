using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmExpiryWarnings : Form
    {
        private DataGridView dgvWarnings;

        public frmExpiryWarnings()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 500);
            this.Text = "Kritik SKT ve Stok Uyarıları";

            var lblTitle = new Label { Text = "SKT YAKLAŞAN / GEÇEN ÜRÜNLER", Font = ThemeManager.HeaderFont, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter, ForeColor = ThemeManager.DangerColor };
            this.Controls.Add(lblTitle);

            dgvWarnings = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = ThemeManager.BackgroundColor
            };
            this.Controls.Add(dgvWarnings);
        }

        private void LoadData()
        {
            using (var db = new MarketDbContext())
            {
                var threshold = DateTime.Now.AddDays(15);
                dgvWarnings.DataSource = db.Products
                    .Where(p => p.IsActive && p.ExpiryDate != null && p.ExpiryDate <= threshold)
                    .Select(p => new {
                        Barkod = p.Barcode,
                        UrunAdi = p.Name,
                        SonKullanma = p.ExpiryDate,
                        Durum = p.ExpiryDate < DateTime.Now ? "SKT GEÇMİŞ!" : "YAKLAŞIYOR"
                    }).ToList();
            }
        }
    }
}
