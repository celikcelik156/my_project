using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Models;
using MarketAutomation.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MarketAutomation.Forms
{
    public class frmPurchaseInvoice : Form
    {
        private ComboBox cbSupplier;
        private TextBox txtBarcode, txtQuantity, txtPurchasePrice;
        private Label lblTotalInvoice;
        private Button btnAdd, btnSaveInvoice, btnClose;
        private DataGridView dgvInvoiceLines;

        private List<InvoiceLineModel> invoiceLines = new List<InvoiceLineModel>();

        public frmPurchaseInvoice()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            LoadSuppliers();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1000, 700);
            this.Text = "Alış Faturası (Toplu Stok Girişi)";

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 130, BackColor = ThemeManager.SurfaceColor };
            this.Controls.Add(panelTop);

            var lblTitle = new Label { Text = "ALIŞ FATURASI İŞLEME", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, 10) };
            panelTop.Controls.Add(lblTitle);

            panelTop.Controls.Add(new Label { Text = "Tedarikçi:", Location = new Point(20, 60), AutoSize = true });
            cbSupplier = new ComboBox { Location = new Point(150, 55), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            panelTop.Controls.Add(cbSupplier);

            var panelLeft = new Panel { Dock = DockStyle.Left, Width = 450, Padding = new Padding(20) };
            this.Controls.Add(panelLeft);

            int yPos = 20;

            panelLeft.Controls.Add(new Label { Text = "Barkod Okut/Yaz:", AutoSize = true, Location = new Point(20, yPos) });
            txtBarcode = new TextBox { Location = new Point(20, yPos + 40), Width = 300, Font = new Font("Segoe UI", 14) };
            txtBarcode.KeyDown += TxtBarcode_KeyDown;
            panelLeft.Controls.Add(txtBarcode);
            yPos += 90;

            panelLeft.Controls.Add(new Label { Text = "Miktar:", AutoSize = true, Location = new Point(20, yPos) });
            txtQuantity = new TextBox { Location = new Point(20, yPos + 40), Width = 140, Text = "1" };
            panelLeft.Controls.Add(txtQuantity);

            panelLeft.Controls.Add(new Label { Text = "Birim Alış (TL):", AutoSize = true, Location = new Point(220, yPos) });
            txtPurchasePrice = new TextBox { Location = new Point(220, yPos + 40), Width = 140 };
            panelLeft.Controls.Add(txtPurchasePrice);
            yPos += 70;

            btnAdd = new Button { Text = "LİSTEYE EKLE", Location = new Point(20, yPos), Width = 300, Height = 45, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnAdd.Click += BtnAdd_Click;
            panelLeft.Controls.Add(btnAdd);
            yPos += 100;

            lblTotalInvoice = new Label { Text = "TOPLAM: 0.00 TL", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = ThemeManager.DangerColor, AutoSize = true, Location = new Point(20, yPos) };
            panelLeft.Controls.Add(lblTotalInvoice);
            yPos += 50;

            btnSaveInvoice = new Button { Text = "FATURAYI ONAYLA (Stokları Artır)", Location = new Point(20, yPos), Width = 300, Height = 60, BackColor = ThemeManager.SuccessColor, ForeColor = Color.White };
            btnSaveInvoice.Click += BtnSaveInvoice_Click;
            panelLeft.Controls.Add(btnSaveInvoice);
            yPos += 75;

            btnClose = new Button { Text = "KAPAT", Location = new Point(20, yPos), Width = 300, Height = 45, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();
            panelLeft.Controls.Add(btnClose);

            dgvInvoiceLines = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = ThemeManager.BackgroundColor
            };
            this.Controls.Add(dgvInvoiceLines);
            dgvInvoiceLines.BringToFront();
        }

        private void LoadSuppliers()
        {
            using (var db = new MarketDbContext())
            {
                cbSupplier.DataSource = db.Suppliers.ToList();
                cbSupplier.DisplayMember = "CompanyName";
                cbSupplier.ValueMember = "Id";
            }
        }

        private void TxtBarcode_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (!string.IsNullOrWhiteSpace(txtBarcode.Text))
                {
                    using (var db = new MarketDbContext())
                    {
                        var prod = db.Products.FirstOrDefault(p => p.Barcode == txtBarcode.Text.Trim() && p.IsActive);
                        if (prod != null)
                        {
                            txtPurchasePrice.Text = prod.PurchasePrice.ToString("0.##");
                            txtQuantity.Focus();
                            txtQuantity.SelectAll();
                        }
                        else
                        {
                            MessageBox.Show("Ürün bulunamadı!", "Hata");
                        }
                    }
                }
            }
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (cbSupplier.SelectedValue == null) { MessageBox.Show("Tedarikçi seçiniz."); return; }
            if (string.IsNullOrWhiteSpace(txtBarcode.Text)) return;

            decimal qty = decimal.TryParse(txtQuantity.Text, out var q) ? q : 0;
            decimal price = decimal.TryParse(txtPurchasePrice.Text, out var p) ? p : 0;

            if (qty <= 0) return;

            using (var db = new MarketDbContext())
            {
                var prod = db.Products.FirstOrDefault(pr => pr.Barcode == txtBarcode.Text.Trim() && pr.IsActive);
                if (prod != null)
                {
                    invoiceLines.Add(new InvoiceLineModel
                    {
                        ProductId = prod.Id,
                        Barcode = prod.Barcode,
                        ProductName = prod.Name,
                        Quantity = qty,
                        UnitPrice = price
                    });
                    RefreshGrid();
                    txtBarcode.Clear();
                    txtPurchasePrice.Clear();
                    txtQuantity.Text = "1";
                    txtBarcode.Focus();
                }
            }
        }

        private void RefreshGrid()
        {
            dgvInvoiceLines.DataSource = null;
            dgvInvoiceLines.DataSource = invoiceLines;
            
            decimal total = invoiceLines.Sum(x => x.SubTotal);
            lblTotalInvoice.Text = $"TOPLAM: {total:C2}";
        }

        private void BtnSaveInvoice_Click(object? sender, EventArgs e)
        {
            if (!invoiceLines.Any()) return;
            if (cbSupplier.SelectedValue == null) return;

            int supplierId = (int)cbSupplier.SelectedValue;
            decimal totalAmount = invoiceLines.Sum(x => x.SubTotal);

            if (MessageBox.Show("Faturayı onaylamak ve stokları artırmak istiyor musunuz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Cursor = Cursors.WaitCursor;
                using (var db = new MarketDbContext())
                {
                    var supplier = db.Suppliers.Find(supplierId);
                    if (supplier != null)
                    {
                        supplier.Balance += totalAmount;
                    }

                    foreach (var line in invoiceLines)
                    {
                        var prod = db.Products.Find(line.ProductId);
                        if (prod != null)
                        {
                            // Stok arttırımı
                            prod.StockQuantity += line.Quantity;
                            // En son alınan fiyat geçerli olsun
                            prod.PurchasePrice = line.UnitPrice;
                        }
                    }

                    Logger.LogActivity("INVOICE_IN", $"{supplier?.CompanyName} firmasından {totalAmount:C2} faturayla stok girişi yapıldı.");
                    db.SaveChanges();
                }
                this.Cursor = Cursors.Default;
                
                MessageBox.Show("Fatura başarıyla işlendi ve stoklar artırıldı.", "Bilgi");
                invoiceLines.Clear();
                RefreshGrid();
            }
        }

        public class InvoiceLineModel
        {
            public int ProductId { get; set; }
            public string Barcode { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public decimal Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal SubTotal => Quantity * UnitPrice;
        }
    }
}
