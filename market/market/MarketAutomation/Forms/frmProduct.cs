using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Models;
using MarketAutomation.Helpers;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace MarketAutomation.Forms
{
    public class frmProduct : Form
    {
        private DataGridView dgvProducts;
        private TextBox txtBarcode, txtName, txtPurchasePrice, txtSalePrice, txtStock;
        private ComboBox cbCategory, cbSupplier;
        private Button btnSave, btnDelete, btnClose;
        private CheckBox chkIsFavorite;
        private PictureBox picProductImage;
        private Button btnSelectImage;
        private string? currentImagePath = null;
        private int selectedId = 0;

        public frmProduct()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            LoadCombos();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1100, 750);
            this.Text = "Ürün Yönetimi";

            var panelLeft = new Panel { Dock = DockStyle.Left, Width = 450, Padding = new Padding(20), AutoScroll = true };
            this.Controls.Add(panelLeft);

            var lblTitle = new Label { Text = "ÜRÜN BİLGİLERİ", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, 20) };
            panelLeft.Controls.Add(lblTitle);

            int yPos = 70;
            
            // Barcode
            panelLeft.Controls.Add(new Label { Text = "Barkod:", AutoSize = true, Location = new Point(20, yPos) });
            txtBarcode = new TextBox { Location = new Point(20, yPos + 40), Width = 300 };
            panelLeft.Controls.Add(txtBarcode);
            yPos += 90;

            // Name
            panelLeft.Controls.Add(new Label { Text = "Ürün Adı:", AutoSize = true, Location = new Point(20, yPos) });
            txtName = new TextBox { Location = new Point(20, yPos + 40), Width = 300 };
            panelLeft.Controls.Add(txtName);
            yPos += 90;

            // Category
            panelLeft.Controls.Add(new Label { Text = "Kategori:", AutoSize = true, Location = new Point(20, yPos) });
            cbCategory = new ComboBox { Location = new Point(20, yPos + 40), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            panelLeft.Controls.Add(cbCategory);
            yPos += 90;

            // Supplier
            panelLeft.Controls.Add(new Label { Text = "Tedarikçi:", AutoSize = true, Location = new Point(20, yPos) });
            cbSupplier = new ComboBox { Location = new Point(20, yPos + 40), Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            panelLeft.Controls.Add(cbSupplier);
            yPos += 90;

            // Purchase Price
            panelLeft.Controls.Add(new Label { Text = "Alış Fiyatı:", AutoSize = true, Location = new Point(20, yPos) });
            txtPurchasePrice = new TextBox { Location = new Point(20, yPos + 40), Width = 140 };
            panelLeft.Controls.Add(txtPurchasePrice);

            // Sale Price
            panelLeft.Controls.Add(new Label { Text = "Satış Fiyatı:", AutoSize = true, Location = new Point(240, yPos) });
            txtSalePrice = new TextBox { Location = new Point(240, yPos + 40), Width = 140 };
            panelLeft.Controls.Add(txtSalePrice);
            yPos += 90;

            // Stock
            panelLeft.Controls.Add(new Label { Text = "Mevcut Stok:", AutoSize = true, Location = new Point(20, yPos) });
            txtStock = new TextBox { Location = new Point(20, yPos + 40), Width = 140 };
            panelLeft.Controls.Add(txtStock);
            yPos += 90;

            // Favorite CheckBox
            chkIsFavorite = new CheckBox { Text = "Favori Ürün (Hızlı POS)", AutoSize = true, Location = new Point(20, yPos), Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeManager.PrimaryColor };
            panelLeft.Controls.Add(chkIsFavorite);
            yPos += 45;

            // Product Image
            panelLeft.Controls.Add(new Label { Text = "Ürün Resmi:", AutoSize = true, Location = new Point(20, yPos) });
            picProductImage = new PictureBox { Location = new Point(140, yPos), Width = 100, Height = 100, BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom, BackColor = Color.White };
            panelLeft.Controls.Add(picProductImage);
            
            btnSelectImage = new Button { Text = "Resim Seç", Location = new Point(260, yPos + 30), Width = 100, Height = 40, BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnSelectImage.FlatAppearance.BorderSize = 0;
            btnSelectImage.Click += BtnSelectImage_Click;
            panelLeft.Controls.Add(btnSelectImage);
            yPos += 130;

            // Buttons
            btnSave = new Button { Text = "KAYDET", Location = new Point(20, yPos), Width = 140, Height = 45, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnSave.Click += BtnSave_Click;

            btnDelete = new Button { Text = "SİL (PASİF)", Location = new Point(240, yPos), Width = 140, Height = 45, BackColor = ThemeManager.DangerColor, ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            yPos += 55;

            btnClose = new Button { Text = "KAPAT", Location = new Point(20, yPos), Width = 360, Height = 45, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();
            yPos += 55;

            var btnExport = new Button { Text = "EXCEL'E AKTAR (Yedek Al)", Location = new Point(20, yPos), Width = 380, Height = 45, BackColor = Color.FromArgb(33, 115, 70), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnExport.Click += BtnExport_Click;

            var btnImport = new Button { Text = "EXCEL'DEN YÜKLE (Güncelle)", Location = new Point(20, yPos + 55), Width = 380, Height = 45, BackColor = Color.DarkOrange, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnImport.Click += BtnImport_Click;

            panelLeft.Controls.AddRange(new Control[] { btnSave, btnDelete, btnClose, btnExport, btnImport });

            // Grid
            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = ThemeManager.BackgroundColor
            };
            dgvProducts.CellDoubleClick += Dgv_CellDoubleClick;
            this.Controls.Add(dgvProducts);
            dgvProducts.BringToFront();
        }

        private void LoadCombos()
        {
            using (var db = new MarketDbContext())
            {
                cbCategory.DataSource = db.Categories.ToList();
                cbCategory.DisplayMember = "Name";
                cbCategory.ValueMember = "Id";

                var suppliers = db.Suppliers.ToList();
                suppliers.Insert(0, new Supplier { Id = 0, CompanyName = "Seçiniz" });
                cbSupplier.DataSource = suppliers;
                cbSupplier.DisplayMember = "CompanyName";
                cbSupplier.ValueMember = "Id";
            }
        }

        private void LoadData()
        {
            using (var db = new MarketDbContext())
            {
                dgvProducts.DataSource = db.Products
                    .Include(p => p.Category)
                    .Where(p => p.IsActive)
                    .Select(p => new {
                        p.Id,
                        Barkod = p.Barcode,
                        UrunAdi = p.Name,
                        Kategori = p.Category.Name,
                        SatisFiyati = p.SalePrice,
                        Stok = p.StockQuantity,
                        Favori = p.IsFavorite ? "EVET" : "HAYIR"
                    }).ToList();
            }
        }

        private Image LoadImageNoLock(string path)
        {
            using (var ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(path)))
            {
                return Image.FromStream(ms);
            }
        }

        private void BtnSelectImage_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Resim Dosyaları|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                    if (!System.IO.Directory.Exists(imagesDir)) System.IO.Directory.CreateDirectory(imagesDir);
                    
                    string ext = System.IO.Path.GetExtension(ofd.FileName);
                    string newFileName = $"PROD_{DateTime.Now.Ticks}{ext}";
                    string newPath = System.IO.Path.Combine(imagesDir, newFileName);
                    
                    System.IO.File.Copy(ofd.FileName, newPath, true);
                    currentImagePath = newPath;
                    picProductImage.Image = LoadImageNoLock(newPath);
                }
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtBarcode.Text)) return;

            decimal buyPrice = decimal.TryParse(txtPurchasePrice.Text, out var b) ? b : 0;
            decimal sellPrice = decimal.TryParse(txtSalePrice.Text, out var s) ? s : 0;
            decimal stock = decimal.TryParse(txtStock.Text, out var st) ? st : 0;
            int catId = cbCategory.SelectedValue != null ? Convert.ToInt32(cbCategory.SelectedValue) : 0;
            int? supId = cbSupplier.SelectedValue != null && Convert.ToInt32(cbSupplier.SelectedValue) != 0 ? (int?)Convert.ToInt32(cbSupplier.SelectedValue) : null;

            using (var db = new MarketDbContext())
            {
                if (selectedId == 0)
                {
                    db.Products.Add(new Product 
                    { 
                        Barcode = txtBarcode.Text,
                        Name = txtName.Text,
                        CategoryId = catId,
                        SupplierId = supId,
                        PurchasePrice = buyPrice,
                        SalePrice = sellPrice,
                        StockQuantity = stock,
                        IsFavorite = chkIsFavorite.Checked,
                        ImagePath = currentImagePath
                    });
                    Logger.LogActivity("PRODUCT_ADD", $"{txtName.Text} eklendi.");
                }
                else
                {
                    var p = db.Products.Find(selectedId);
                    if (p != null)
                    {
                        p.Barcode = txtBarcode.Text;
                        p.Name = txtName.Text;
                        p.CategoryId = catId;
                        p.SupplierId = supId;
                        p.PurchasePrice = buyPrice;
                        p.SalePrice = sellPrice;
                        p.StockQuantity = stock;
                        p.IsFavorite = chkIsFavorite.Checked;
                        p.ImagePath = currentImagePath;
                        Logger.LogActivity("PRODUCT_UPDATE", $"{txtName.Text} güncellendi.");
                    }
                }
                db.SaveChanges();
            }
            ClearForm();
            LoadData();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (selectedId == 0) return;
            if (MessageBox.Show("Ürünü silmek (pasife almak) istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var db = new MarketDbContext())
                {
                    var p = db.Products.Find(selectedId);
                    if (p != null)
                    {
                        p.IsActive = false; // Soft delete
                        db.SaveChanges();
                        Logger.LogActivity("PRODUCT_DELETE", $"{p.Name} pasife alındı.");
                    }
                }
                ClearForm();
                LoadData();
            }
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = Convert.ToInt32(dgvProducts.Rows[e.RowIndex].Cells["Id"].Value);
                using (var db = new MarketDbContext())
                {
                    var p = db.Products.Find(selectedId);
                    if (p != null)
                    {
                        txtBarcode.Text = p.Barcode;
                        txtName.Text = p.Name;
                        txtPurchasePrice.Text = p.PurchasePrice.ToString("0.##");
                        txtSalePrice.Text = p.SalePrice.ToString("0.##");
                        txtStock.Text = p.StockQuantity.ToString("0.##");
                        cbCategory.SelectedValue = p.CategoryId;
                        if (p.SupplierId.HasValue) cbSupplier.SelectedValue = p.SupplierId.Value;
                        else cbSupplier.SelectedValue = 0;
                        chkIsFavorite.Checked = p.IsFavorite;
                        currentImagePath = p.ImagePath;
                        if (!string.IsNullOrEmpty(p.ImagePath) && System.IO.File.Exists(p.ImagePath))
                        {
                            picProductImage.Image = LoadImageNoLock(p.ImagePath);
                        }
                        else 
                        {
                            picProductImage.Image = null;
                        }
                    }
                }
            }
        }

        private void ClearForm()
        {
            selectedId = 0;
            txtBarcode.Clear();
            txtName.Clear();
            txtPurchasePrice.Clear();
            txtSalePrice.Clear();
            txtStock.Clear();
            chkIsFavorite.Checked = false;
            picProductImage.Image = null;
            currentImagePath = null;
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Çalışma Kitabı|*.xlsx", FileName = "Urunler_Yedek_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Ürünler");
                            ws.Cell(1, 1).Value = "Barkod";
                            ws.Cell(1, 2).Value = "Ürün Adı";
                            ws.Cell(1, 3).Value = "Alış Fiyatı";
                            ws.Cell(1, 4).Value = "Satış Fiyatı";
                            ws.Cell(1, 5).Value = "Stok";

                            // Stil
                            var header = ws.Range(1, 1, 1, 5);
                            header.Style.Font.Bold = true;
                            header.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                            header.Style.Font.FontColor = XLColor.White;

                            using (var db = new MarketDbContext())
                            {
                                var products = db.Products.Where(p => p.IsActive).ToList();
                                int row = 2;
                                foreach (var p in products)
                                {
                                    ws.Cell(row, 1).Value = p.Barcode;
                                    ws.Cell(row, 2).Value = p.Name;
                                    ws.Cell(row, 3).Value = p.PurchasePrice;
                                    ws.Cell(row, 4).Value = p.SalePrice;
                                    ws.Cell(row, 5).Value = p.StockQuantity;
                                    row++;
                                }
                            }
                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            MessageBox.Show("Ürünler başarıyla Excel'e aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Excel aktarımı sırasında hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void BtnImport_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("Excel'den içe aktarma işlemi, barkodu eşleşen ürünlerin TEK SEFERDE fiyat ve stoklarını güncelleyecek, eşleşmeyenleri yeni ürün olarak ekleyecektir.\nDevam edilsin mi?", "Toplu Güncelleme Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "Excel Çalışma Kitabı|*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var db = new MarketDbContext())
                        using (var workbook = new XLWorkbook(ofd.FileName))
                        {
                            var ws = workbook.Worksheet(1);
                            var rows = ws.RangeUsed().RowsUsed().Skip(1); // Başlığı atla
                            
                            int updatedCount = 0;
                            int addedCount = 0;
                            var defaultCatId = db.Categories.FirstOrDefault()?.Id ?? 0;

                            foreach (var row in rows)
                            {
                                string barcode = row.Cell(1).Value.ToString().Trim();
                                string name = row.Cell(2).Value.ToString().Trim();
                                
                                if (string.IsNullOrEmpty(barcode) || string.IsNullOrEmpty(name)) continue;

                                decimal.TryParse(row.Cell(3).Value.ToString(), out decimal pPrice);
                                decimal.TryParse(row.Cell(4).Value.ToString(), out decimal sPrice);
                                decimal.TryParse(row.Cell(5).Value.ToString(), out decimal stock);

                                var existing = db.Products.FirstOrDefault(p => p.Barcode == barcode);
                                if (existing != null)
                                {
                                    existing.Name = name;
                                    existing.PurchasePrice = pPrice;
                                    existing.SalePrice = sPrice;
                                    existing.StockQuantity = stock;
                                    existing.IsActive = true;
                                    updatedCount++;
                                }
                                else
                                {
                                    if(defaultCatId == 0) continue; // Kategori yoksa yeni ekleyemeyiz

                                    db.Products.Add(new Product {
                                        Barcode = barcode,
                                        Name = name,
                                        PurchasePrice = pPrice,
                                        SalePrice = sPrice,
                                        StockQuantity = stock,
                                        CategoryId = defaultCatId,
                                        SupplierId = null,
                                        IsFavorite = false,
                                        IsActive = true
                                    });
                                    addedCount++;
                                }
                            }

                            db.SaveChanges();
                            Logger.LogActivity("EXCEL_SYNC", $"Excel senkronizasyonu yapıldı. {updatedCount} güncellendi, {addedCount} eklendi.");
                            MessageBox.Show($"Senkronizasyon Tamamlandı!\nGüncellenen Ürün: {updatedCount}\nYeni Eklenen: {addedCount}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Excel yüklemesi sırasında hata oluştu. " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
