using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmInventoryCount : Form
    {
        private DataGridView dgvInventory;
        private TextBox txtBarcode, txtPhysicalStock;
        private Button btnUpdateStock, btnClose;
        private int selectedProductId = 0;

        public frmInventoryCount()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(950, 650);
            this.Text = "Envanter (Sayım) Yönetimi";

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = ThemeManager.SurfaceColor, Padding = new Padding(20) };
            this.Controls.Add(panelTop);

            var lblTitle = new Label { Text = "STOK SAYIMI VE GÜNCELLEME", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, 10) };
            panelTop.Controls.Add(lblTitle);

            panelTop.Controls.Add(new Label { Text = "Barkod:", AutoSize = true, Location = new Point(20, 60) });
            txtBarcode = new TextBox { Location = new Point(130, 55), Width = 150 };
            txtBarcode.KeyDown += TxtBarcode_KeyDown;
            panelTop.Controls.Add(txtBarcode);

            panelTop.Controls.Add(new Label { Text = "Fiziksel Kalan:", AutoSize = true, Location = new Point(320, 60) });
            txtPhysicalStock = new TextBox { Location = new Point(460, 55), Width = 100 };
            panelTop.Controls.Add(txtPhysicalStock);

            btnUpdateStock = new Button { Text = "STOK EŞİTLE", Location = new Point(600, 50), Width = 150, Height = 35, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnUpdateStock.Click += BtnUpdateStock_Click;
            panelTop.Controls.Add(btnUpdateStock);

            btnClose = new Button { Text = "KAPAT", Location = new Point(780, 50), Width = 150, Height = 35, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();
            panelTop.Controls.Add(btnClose);

            dgvInventory = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = ThemeManager.BackgroundColor
            };
            dgvInventory.CellClick += DgvInventory_CellClick;
            this.Controls.Add(dgvInventory);
            dgvInventory.BringToFront();
        }

        private void LoadData()
        {
            using (var db = new MarketDbContext())
            {
                dgvInventory.DataSource = db.Products.Where(p => p.IsActive).Select(p => new {
                    p.Id,
                    Barkod = p.Barcode,
                    UrunAdi = p.Name,
                    SistemStogu = p.StockQuantity
                }).ToList();
            }
        }

        private void TxtBarcode_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                using (var db = new MarketDbContext())
                {
                    var p = db.Products.FirstOrDefault(x => x.Barcode == txtBarcode.Text && x.IsActive);
                    if (p != null)
                    {
                        selectedProductId = p.Id;
                        txtPhysicalStock.Focus();
                    }
                    else
                    {
                        MessageBox.Show("Ürün bulunamadı!");
                    }
                }
            }
        }

        private void DgvInventory_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedProductId = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["Id"].Value);
                txtBarcode.Text = dgvInventory.Rows[e.RowIndex].Cells["Barkod"].Value.ToString();
                txtPhysicalStock.Text = dgvInventory.Rows[e.RowIndex].Cells["SistemStogu"].Value.ToString();
            }
        }

        private void BtnUpdateStock_Click(object? sender, EventArgs e)
        {
            if (selectedProductId == 0) return;
            if (decimal.TryParse(txtPhysicalStock.Text, out decimal newStock))
            {
                using (var db = new MarketDbContext())
                {
                    var p = db.Products.Find(selectedProductId);
                    if (p != null)
                    {
                        decimal diff = newStock - p.StockQuantity;
                        if (diff != 0)
                        {
                            Logger.LogActivity("INVENTORY_COUNT", $"{p.Name} stok eşitlemesi. Eski: {p.StockQuantity}, Yeni: {newStock}, Fark: {diff}");
                            p.StockQuantity = newStock;
                            db.SaveChanges();
                            MessageBox.Show("Stok güncellendi.");
                        }
                    }
                }
                txtBarcode.Clear();
                txtPhysicalStock.Clear();
                selectedProductId = 0;
                LoadData();
            }
        }
    }
}
