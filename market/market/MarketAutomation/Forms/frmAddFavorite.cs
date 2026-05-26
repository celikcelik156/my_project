using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmAddFavorite : Form
    {
        private DataGridView dgvProducts;
        private TextBox txtSearch;

        public frmAddFavorite()
        {
            this.Size = new Size(800, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Hızlı POS Favori Ürünleri Belirle";
            ThemeManager.ApplyTheme(this);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(20) };
            this.Controls.Add(pnlTop);

            pnlTop.Controls.Add(new Label { Text = "Ürün Adı veya Barkod Ara:", Location = new Point(20, 35), AutoSize = true });
            txtSearch = new TextBox { Location = new Point(250, 30), Width = 400, Font = new Font("Segoe UI", 16) };
            txtSearch.TextChanged += (s, e) => LoadData(txtSearch.Text);
            pnlTop.Controls.Add(txtSearch);

            var lblInfo = new Label { Text = "Çift tıklayarak favoriye ekleyin veya çıkarın.", Location = new Point(20, 70), ForeColor = ThemeManager.PrimaryColor, AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            pnlTop.Controls.Add(lblInfo);

            dgvProducts = new DataGridView
            {
                Location = new Point(0, 100),
                Size = new Size(800, 450), // Responsive on Anchor
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowTemplate = { Height = 45 },
                Font = new Font("Segoe UI", 14)
            };
            dgvProducts.CellDoubleClick += Dgv_DoubleClick;
            this.Controls.Add(dgvProducts);

            LoadData("");
        }

        private void LoadData(string search)
        {
            using (var db = new MarketDbContext())
            {
                var query = db.Products.Where(p => p.IsActive);
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(p => p.Name.ToLower().Contains(search.ToLower()) || p.Barcode.Contains(search));

                dgvProducts.DataSource = query.Select(p => new {
                    p.Id,
                    Barkod = p.Barcode,
                    UrunAdi = p.Name,
                    Fiyat = p.SalePrice,
                    FavoriMi = p.IsFavorite ? "★ EVET" : "HAYIR"
                }).Take(100).ToList();
            }
        }

        private void Dgv_DoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                int pId = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["Id"].Value);
                using (var db = new MarketDbContext())
                {
                    var p = db.Products.Find(pId);
                    if (p != null)
                    {
                        p.IsFavorite = !p.IsFavorite;
                        db.SaveChanges();
                    }
                }
                LoadData(txtSearch.Text);
            }
        }
    }
}
