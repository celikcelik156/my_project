using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Models;
using MarketAutomation.Helpers;
using System.Collections.Generic;
using System.IO.Ports;
using Microsoft.EntityFrameworkCore;

namespace MarketAutomation.Forms
{
    public class frmPOS : Form
    {
        private DataGridView dgvCart;
        private TextBox txtBarcode;
        private TextBox txtQuantity;
        private ComboBox cboComPorts;
        private Label lblTotal, lblDiscount, lblGrandTotal, lblCustomer;
        private Button btnCash, btnCard, btnCredit, btnCancelLine, btnCancelAll, btnSelectCustomer, btnClose, btnPlus, btnMinus, btnItemPlus, btnItemMinus, btnBarcodeKbd, btnSearch;
        private Panel panelRight, panelTop;
        private FlowLayoutPanel flpProducts;
        private FlowLayoutPanel flpCategories;
        private PictureBox pbxProductPreview;
        private Label lblDetailName, lblDetailPrice, lblDetailCategory;
        private bool isSearchMode = false;

        private System.ComponentModel.BindingList<POSItem> cart = new System.ComponentModel.BindingList<POSItem>();
        private Customer? selectedCustomer = null;
        private string appliedCampaigns = "";

        public frmPOS()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            ApplyCustomPOSTheme();
            LoadCategories();
            LoadQuickProducts(null);
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1280, 800);
            this.WindowState = FormWindowState.Maximized;
            this.Text = "Hızlı Satış (POS)";

            // GÜMÜŞ/KOYU TEMA ARKA PLANI
            this.BackColor = Color.FromArgb(240, 240, 245);

            // ÜST PANEL (Barkod Okutma Alanı)
            panelTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = ThemeManager.PrimaryColor };
            this.Controls.Add(panelTop);

            Label lblBarcode = new Label { Text = "BARKOD:", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(20, 38) };
            panelTop.Controls.Add(lblBarcode);
            txtBarcode = new TextBox { Location = new Point(180, 38), Width = 400, Font = new Font("Segoe UI", 24, FontStyle.Bold), Tag = "NO_NAV" };
            txtBarcode.KeyDown += TxtBarcode_KeyDown;
            txtBarcode.GotFocus += (s, e) => { if (txtBarcode.Tag?.ToString() != "NO_SELECT") txtBarcode.SelectAll(); };
            panelTop.Controls.Add(txtBarcode);

            btnBarcodeKbd = new Button { Text = "⌨️", Location = new Point(590, 38), Width = 60, Height = 58, BackColor = Color.FromArgb(52, 152, 219), ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnBarcodeKbd.Click += (s, e) => {
                using (var kbd = new frmKeyboard(txtBarcode.Text)) {
                    if (kbd.ShowDialog() == DialogResult.OK) {
                        txtBarcode.Text = kbd.InputText;
                        txtBarcode.Focus();
                        SendKeys.Send("{ENTER}");
                    }
                }
            };
            panelTop.Controls.Add(btnBarcodeKbd);

            btnSearch = new Button { Text = "🔍 ARAMA", Location = new Point(660, 38), Width = 120, Height = 58, BackColor = Color.DarkOrange, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnSearch.Click += (s, e) => {
                isSearchMode = !isSearchMode;
                if (isSearchMode) {
                    btnSearch.BackColor = Color.Green;
                    txtBarcode.BackColor = Color.LightGreen;
                    txtBarcode.Focus();
                } else {
                    btnSearch.BackColor = Color.DarkOrange;
                    txtBarcode.BackColor = Color.White;
                    txtBarcode.Focus();
                }
            };
            panelTop.Controls.Add(btnSearch);

            Label lblQty = new Label { Text = "MİKTAR:", Font = new Font("Segoe UI", 13, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(790, 10) };
            panelTop.Controls.Add(lblQty);

            btnMinus = new Button { Text = "-", Location = new Point(790, 38), Width = 50, Height = 58, BackColor = Color.Tomato, ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnMinus.Click += (s, e) => { 
                if (decimal.TryParse(txtQuantity.Text, out decimal v) && v > 1) txtQuantity.Text = (v - 1).ToString(); 
                if (!dgvCart.IsCurrentCellInEditMode) txtBarcode.Focus(); 
            };
            panelTop.Controls.Add(btnMinus);

            txtQuantity = new TextBox { Location = new Point(840, 38), Width = 100, Font = new Font("Segoe UI", 24, FontStyle.Bold), Text = "1", Tag = "NO_NAV", TextAlign = HorizontalAlignment.Center };
            txtQuantity.KeyDown += TxtQuantity_KeyDown;
            txtQuantity.DoubleClick += (s, e) => {
                using (var kbd = new frmKeyboard(txtQuantity.Text, isNumeric: true)) {
                    if (kbd.ShowDialog() == DialogResult.OK) txtQuantity.Text = kbd.InputText;
                }
            };
            panelTop.Controls.Add(txtQuantity);

            btnPlus = new Button { Text = "+", Location = new Point(940, 38), Width = 50, Height = 58, BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 18, FontStyle.Bold), FlatStyle = FlatStyle.Flat };
            btnPlus.Click += (s, e) => { 
                if (decimal.TryParse(txtQuantity.Text, out decimal v)) txtQuantity.Text = (v + 1).ToString(); 
                if (!dgvCart.IsCurrentCellInEditMode) txtBarcode.Focus(); 
            };
            panelTop.Controls.Add(btnPlus);

            Label lblPort = new Label { Text = "POS PORT:", Font = new Font("Segoe UI", 10, FontStyle.Bold), ForeColor = Color.White, AutoSize = true, Location = new Point(1010, 10) };
            panelTop.Controls.Add(lblPort);
            cboComPorts = new ComboBox { Location = new Point(1010, 38), Width = 100, Font = new Font("Segoe UI", 14), DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (string p in SerialPort.GetPortNames()) cboComPorts.Items.Add(p);
            if (cboComPorts.Items.Count > 0) cboComPorts.SelectedIndex = 0;
            else cboComPorts.Items.Add("COM3");
            panelTop.Controls.Add(cboComPorts);

            // SAĞ PANEL (Ödeme ve Toplamlar)
            panelRight = new Panel { Dock = DockStyle.Right, Width = 400, BackColor = Color.White, Padding = new Padding(20) };
            this.Controls.Add(panelRight);

            int yPos = 20;

            // Label lblPreviewTitle = new Label { Text = "ÜRÜN ÖNİZLEME", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = ThemeManager.PrimaryColor, AutoSize = true, Location = new Point(20, yPos) };
            // panelRight.Controls.Add(lblPreviewTitle); // KALDIRILDI
            // yPos += 30;

            // Ürün Önizleme Görseli
            // pbxProductPreview = new PictureBox {
            //     Size = new Size(360, 200),
            //     Location = new Point(20, yPos),
            //     SizeMode = PictureBoxSizeMode.Zoom,
            //     BackColor = Color.FromArgb(245, 245, 250),
            //     BorderStyle = BorderStyle.FixedSingle
            // };
            // panelRight.Controls.Add(pbxProductPreview); // KALDIRILDI
            // yPos += 240; // Eskisine göre boşluk bırakıldı);
            // yPos += 210;

            yPos = 15;

            lblCustomer = new Label { Text = "Müşteri: STANDART (Perakende)", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = ThemeManager.SecondaryColor, AutoSize = true, Location = new Point(20, yPos) };
            panelRight.Controls.Add(lblCustomer);
            yPos += 50;

            lblTotal = new Label { Text = "ARA TOPLAM: 0.00 TL", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.Gray, AutoSize = true, Location = new Point(20, yPos) };
            panelRight.Controls.Add(lblTotal);
            yPos += 50;

            lblDiscount = new Label { Text = "İNDİRİM: 0.00 TL", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = ThemeManager.DangerColor, AutoSize = true, Location = new Point(20, yPos) };
            panelRight.Controls.Add(lblDiscount);
            yPos += 70;

            lblGrandTotal = new Label { Text = "GENEL TOPLAM:\n0.00 TL", Font = new Font("Segoe UI", 28, FontStyle.Bold), ForeColor = ThemeManager.SuccessColor, AutoSize = true, Location = new Point(20, yPos) };
            panelRight.Controls.Add(lblGrandTotal);
            yPos += 130;

            // DOKUNMATİK BÜYÜK BUTONLAR
            btnCash = new Button { Text = "NAKİT (F10)", Location = new Point(20, yPos), Width = 350, Height = 80, BackColor = ThemeManager.SuccessColor, ForeColor = Color.White, Font = new Font("Segoe UI", 20, FontStyle.Bold) };
            btnCash.Click += BtnCash_Click;
            panelRight.Controls.Add(btnCash);
            yPos += 90;

            btnCard = new Button { Text = "KREDİ KARTI (F11)", Location = new Point(20, yPos), Width = 350, Height = 80, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White, Font = new Font("Segoe UI", 20, FontStyle.Bold) };
            btnCard.Click += BtnCard_Click;
            panelRight.Controls.Add(btnCard);
            yPos += 90;

            btnCredit = new Button { Text = "VERESİYE (BORÇ) YAZ (F12)", Location = new Point(20, yPos), Width = 350, Height = 80, BackColor = Color.DarkOrange, ForeColor = Color.White, Font = new Font("Segoe UI", 20, FontStyle.Bold) };
            btnCredit.Click += BtnCredit_Click;
            panelRight.Controls.Add(btnCredit);
            yPos += 90;

            btnCancelLine = new Button { Text = "SATIR İPTAL (F8)", Location = new Point(20, yPos), Width = 170, Height = 60, BackColor = Color.Orange, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnCancelLine.Click += BtnCancelLine_Click;
            panelRight.Controls.Add(btnCancelLine);

            btnCancelAll = new Button { Text = "FİŞ İPTAL (F9)", Location = new Point(200, yPos), Width = 170, Height = 60, BackColor = ThemeManager.DangerColor, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnCancelAll.Click += BtnCancelAll_Click;
            panelRight.Controls.Add(btnCancelAll);
            yPos += 70;

            btnSelectCustomer = new Button { Text = "MÜŞTERİ SEÇ (Puan/Veresiye)", Location = new Point(20, yPos), Width = 350, Height = 60, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            btnSelectCustomer.Click += BtnSelectCustomer_Click;
            panelRight.Controls.Add(btnSelectCustomer);
            yPos += 70;

            btnClose = new Button { Text = "KASAYI KAPAT (ESC)", Location = new Point(20, yPos), Width = 350, Height = 60, BackColor = Color.DarkGray, ForeColor = Color.White, Font = new Font("Segoe UI", 14, FontStyle.Bold) };
            btnClose.Click += (s, e) => { 
                if (ActiveUser.IsAdmin) this.Close(); 
                else Application.Restart(); 
            };
            panelRight.Controls.Add(btnClose);
 
            // GRID (Sepet) için Sol Panel
            Panel panelLeft = new Panel { Dock = DockStyle.Left, Width = 720, Padding = new Padding(10) };
            this.Controls.Add(panelLeft);
 
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowTemplate = { Height = 65 },
                Font = new Font("Segoe UI", 15),
                EditMode = DataGridViewEditMode.EditOnEnter,
                MultiSelect = false
            };
            
            dgvCart.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle {
                BackColor = ThemeManager.SecondaryColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };
            
            dgvCart.DefaultCellStyle = new DataGridViewCellStyle {
                SelectionBackColor = Color.FromArgb(232, 241, 250),
                SelectionForeColor = Color.Black,
                Padding = new Padding(5)
            };
 
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Barkod", DataPropertyName = "Barcode", Width = 140, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ürün Adı", DataPropertyName = "ProductName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Kategori", DataPropertyName = "CategoryName", Width = 110, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Adet", DataPropertyName = "Quantity", Width = 70, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "B.Fiyat", DataPropertyName = "SalePrice", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2" }, ReadOnly = true });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Toplam", DataPropertyName = "Total", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "C2", Font = new Font("Segoe UI", 15, FontStyle.Bold) }, ReadOnly = true });
 
            dgvCart.CellBeginEdit += DgvCart_CellBeginEdit;
            dgvCart.CellValidating += DgvCart_CellValidating;
            dgvCart.CellEndEdit += DgvCart_CellEndEdit;
            dgvCart.CellDoubleClick += DgvCart_CellDoubleClick;
            dgvCart.EditingControlShowing += DgvCart_EditingControlShowing;
            dgvCart.CellEnter += DgvCart_CellEnter;
            panelLeft.Controls.Add(dgvCart);
            Panel panelCartControls = new Panel { Dock = DockStyle.Bottom, Height = 80, Padding = new Padding(5), BackColor = Color.WhiteSmoke };
            btnItemMinus = new Button { Text = "SEÇİLİ MİKTAR -", Dock = DockStyle.Left, Width = 340, BackColor = Color.OrangeRed, ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Enabled = false };
            btnItemMinus.Click += BtnItemMinus_Click;
            
            btnItemPlus = new Button { Text = "SEÇİLİ MİKTAR +", Dock = DockStyle.Right, Width = 340, BackColor = Color.LimeGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 16, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Enabled = false };
            btnItemPlus.Click += BtnItemPlus_Click;
            
            panelCartControls.Controls.Add(btnItemMinus);
            panelCartControls.Controls.Add(btnItemPlus);
            
            panelLeft.Controls.Add(dgvCart);
            panelLeft.Controls.Add(panelCartControls);

            // KLAVYE / NUMPAD (Orta Alt Alan)
            Panel panelNumPad = new Panel { Dock = DockStyle.Bottom, Height = 320, Padding = new Padding(10), BackColor = Color.WhiteSmoke };
            this.Controls.Add(panelNumPad);
            CreateNumPad(panelNumPad);
            
            // ÜRÜN DETAY ALANI (NumPad'in yanına) - DİKEY TASARIM
            Panel pnlDetails = new Panel { Location = new Point(480, 5), Size = new Size(720, 310), BackColor = Color.Transparent };
            panelNumPad.Controls.Add(pnlDetails);
            
            pbxProductPreview = new PictureBox {
                Size = new Size(180, 180),
                Location = new Point(10, 0),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            pnlDetails.Controls.Add(pbxProductPreview);
            
            lblDetailName = new Label { Text = "", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = ThemeManager.PrimaryColor, Location = new Point(10, 185), Size = new Size(700, 40), AutoEllipsis = true };
            lblDetailPrice = new Label { Text = "", Font = new Font("Segoe UI", 32, FontStyle.Bold), ForeColor = ThemeManager.DangerColor, Location = new Point(10, 225), Size = new Size(700, 60) };
            lblDetailCategory = new Label { Text = "", Font = new Font("Segoe UI", 12, FontStyle.Italic), ForeColor = Color.Gray, Location = new Point(10, 285), Size = new Size(700, 25) };
            
            pnlDetails.Controls.Add(lblDetailName);
            pnlDetails.Controls.Add(lblDetailPrice);
            pnlDetails.Controls.Add(lblDetailCategory);

            // KATEGORİLER (Orta Üst Alan)
            flpCategories = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 85,
                Padding = new Padding(10, 0, 10, 0),
                BackColor = Color.White,
                AutoScroll = false
            };
            this.Controls.Add(flpCategories);

            // HIZLI ÜRÜN BUTONLARI (Kalan Orta Alan)
            flpProducts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(245, 245, 250)
            };
            this.Controls.Add(flpProducts);

            panelLeft.BringToFront();
            flpCategories.BringToFront();
            panelNumPad.BringToFront();
            flpProducts.BringToFront();

            this.KeyPreview = true;
            this.KeyDown += FrmPOS_KeyDown;
        }

        private void ApplyCustomPOSTheme()
        {
            dgvCart.EnableHeadersVisualStyles = false;
            dgvCart.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.SecondaryColor;
            dgvCart.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCart.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            dgvCart.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            panelTop.BackColor = ThemeManager.PrimaryColor;
            foreach (Control c in panelTop.Controls)
            {
                if (c is Label lbl) lbl.ForeColor = Color.White;
            }
        }

        private void LoadCategories()
        {
            flpCategories.Controls.Clear();
            
            Button btnFav = CreateCategoryButton("FAVORİLER", null);
            btnFav.BackColor = ThemeManager.SecondaryColor;
            btnFav.ForeColor = Color.White;
            flpCategories.Controls.Add(btnFav);

            using (var db = new MarketDbContext())
            {
                var categories = db.Categories.ToList();
                foreach (var c in categories) flpCategories.Controls.Add(CreateCategoryButton(c.Name, c.Id));
            }
        }

        private Button CreateCategoryButton(string text, int? categoryId)
        {
            Button btn = new Button
            {
                Text = text, Width = 140, Height = 60, Margin = new Padding(5, 10, 5, 5),
                BackColor = Color.White, ForeColor = ThemeManager.TextColor,
                Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand, Tag = categoryId
            };
            btn.FlatAppearance.BorderColor = Color.LightGray;
            btn.Click += Category_Click;
            return btn;
        }

        private void Category_Click(object? sender, EventArgs e)
        {
            if (sender is Button clickedBtn)
            {
                foreach (Control c in flpCategories.Controls) {
                    if (c is Button b) { b.BackColor = Color.White; b.ForeColor = ThemeManager.TextColor; }
                }
                clickedBtn.BackColor = ThemeManager.SecondaryColor;
                clickedBtn.ForeColor = Color.White;
                int? catId = clickedBtn.Tag as int?;
                LoadQuickProducts(catId);
            }
            txtBarcode.Focus();
        }

        private void LoadQuickProducts(int? categoryId = null)
        {
            flpProducts.Controls.Clear();
            using (var db = new MarketDbContext())
            {
                var query = db.Products.Where(p => p.IsActive);
                
                if (categoryId.HasValue) {
                    query = query.Where(p => p.CategoryId == categoryId.Value);
                } else {
                    query = query.Where(p => p.IsFavorite);

                    // Ekle/Çıkar butonu en başa
                    Button btnAddFav = new Button
                    {
                        Text = "➕ FAVORİ\nEKLE / ÇIKAR",
                        Width = 140, Height = 140, Margin = new Padding(10),
                        BackColor = ThemeManager.DangerColor, ForeColor = Color.White,
                        Font = new Font("Segoe UI", 14, FontStyle.Bold), FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    btnAddFav.Click += (s, e) => {
                        using(var frm = new frmAddFavorite()) { frm.ShowDialog(); }
                        LoadQuickProducts(null);
                    };
                    flpProducts.Controls.Add(btnAddFav);
                }

                var products = query.ToList();
                foreach (var p in products)
                {
                    if (!string.IsNullOrEmpty(p.ImagePath) && System.IO.File.Exists(p.ImagePath))
                    {
                        // Resimli Ürün (Kare Kart Tasarımı: Üstü Resim, Altı Yazı)
                        Panel pnlProduct = new Panel
                        {
                            Width = 140, Height = 140, Margin = new Padding(10),
                            BackColor = Color.White, Cursor = Cursors.Hand,
                            BorderStyle = BorderStyle.FixedSingle
                        };

                        string cleanName = p.Name.Replace("\r", "").Replace("\n", "").Trim();

                        Label lblPrice = new Label
                        {
                            Text = p.SalePrice.ToString("C2"),
                            Dock = DockStyle.Bottom, Height = 25,
                            TextAlign = ContentAlignment.BottomCenter,
                            Font = new Font("Segoe UI", 11, FontStyle.Bold),
                            ForeColor = ThemeManager.DangerColor,
                            BackColor = Color.White,
                            Cursor = Cursors.Hand
                        };

                        Label lblName = new Label
                        {
                            Text = cleanName,
                            Dock = DockStyle.Bottom, Height = 35,
                            AutoEllipsis = true,
                            TextAlign = ContentAlignment.TopCenter,
                            Font = new Font("Segoe UI", 9, FontStyle.Bold),
                            ForeColor = ThemeManager.TextColor,
                            BackColor = Color.White,
                            Cursor = Cursors.Hand
                        };

                        PictureBox picProduct = new PictureBox
                        {
                            Dock = DockStyle.Fill,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            BackColor = Color.White,
                            Cursor = Cursors.Hand
                        };

                        try
                        {
                            using (var ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(p.ImagePath)))
                            {
                                picProduct.Image = Image.FromStream(ms);
                            }
                        }
                        catch { }

                        EventHandler clickHandler = (s, ev) => {
                            ProcessBarcode(p.Barcode, 1);
                            txtBarcode.Focus();
                        };

                        pnlProduct.Click += clickHandler;
                        lblPrice.Click += clickHandler;
                        lblName.Click += clickHandler;
                        picProduct.Click += clickHandler;

                        pnlProduct.Controls.Add(picProduct); // Fill
                        pnlProduct.Controls.Add(lblName); // Bottom (Above Price)
                        pnlProduct.Controls.Add(lblPrice); // Bottom (Lowest)

                        flpProducts.Controls.Add(pnlProduct);
                    }
                    else
                    {
                        // Resimsiz Ürün (Sadece Renkli Buton)
                        string cleanNameBtn = p.Name.Replace("\r", "").Replace("\n", "").Trim();
                        Button btn = new Button
                        {
                            Text = cleanNameBtn + "\n" + p.SalePrice.ToString("C2"),
                            Width = 140, Height = 140, Margin = new Padding(10),
                            BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White,
                            Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat,
                            Cursor = Cursors.Hand, Tag = p.Barcode,
                            TextAlign = ContentAlignment.MiddleCenter
                        };
                        btn.FlatAppearance.BorderSize = 0;
                        btn.Click += QuickProduct_Click;
                        flpProducts.Controls.Add(btn);
                    }
                }
            }
        }

        private void QuickProduct_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string barcode = btn.Tag.ToString();
                ProcessBarcode(barcode, 1);
            }
            txtBarcode.Focus();
        }

        private void CreateNumPad(Panel container)
        {
            string[] keys = { "7", "8", "9", "C", 
                              "4", "5", "6", "Sil", 
                              "1", "2", "3", "ENTER", 
                              "0", "00", "*" }; 
            int btnW = 90, btnH = 65, space = 10;
            int startX = 60, startY = 10;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    int index = i * 4 + j;
                    if (index >= keys.Length) continue;
                    
                    string txt = keys[index];
                    if (string.IsNullOrEmpty(txt)) continue;

                    Button b = new Button
                    {
                        Text = txt,
                        Font = new Font("Segoe UI", 18, FontStyle.Bold),
                        Size = new Size(btnW, btnH),
                        Location = new Point(startX + j * (btnW + space), startY + i * (btnH + space)),
                        BackColor = (txt == "ENTER" ? ThemeManager.SuccessColor : (txt == "C" || txt == "Sil" ? ThemeManager.DangerColor : Color.White)),
                        ForeColor = (txt == "ENTER" || txt == "C" || txt == "Sil" ? Color.White : Color.Black),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };
                    b.FlatAppearance.BorderSize = 1;
                    b.FlatAppearance.BorderColor = Color.LightGray;

                    if (txt == "ENTER")
                    {
                        b.Height = btnH * 2 + space;
                    }
                    b.Click += NumPad_Click;
                    container.Controls.Add(b);
                }
            }
        }

        private void NumPad_Click(object? sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                string key = btn.Text;
                if (key == "C") txtBarcode.Clear();
                else if (key == "Sil")
                {
                    if (txtBarcode.Text.Length > 0)
                        txtBarcode.Text = txtBarcode.Text.Substring(0, txtBarcode.Text.Length - 1);
                }
                else if (key == "ENTER") 
                {
                    KeyEventArgs args = new KeyEventArgs(Keys.Enter);
                    TxtBarcode_KeyDown(txtBarcode, args);
                }
                else 
                {
                    txtBarcode.Text += key;
                }
                txtBarcode.Focus();
                txtBarcode.SelectionStart = txtBarcode.Text.Length;
            }
        }

        private void FrmPOS_KeyDown(object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F10:
                    BtnCash_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.F11:
                    BtnCard_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.F12:
                    BtnCredit_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.F8:
                    BtnCancelLine_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.F9:
                    BtnCancelAll_Click(null, EventArgs.Empty);
                    e.Handled = true; break;
                case Keys.Escape:
                    if (ActiveUser.IsAdmin) this.Close();
                    else Application.Restart();
                    e.Handled = true; break;
            }
            // Miktar alanı odakta ise txtBarcode'a yönlendirme
            if (!e.Handled && this.ActiveControl != txtQuantity)
                txtBarcode.Focus();
        }

        private void TxtBarcode_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true; // ThemeManager'ın TAB handler'ının devreye girmesini engelle

                string input = txtBarcode.Text.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    txtBarcode.Clear();
                    txtBarcode.Focus();
                    return;
                }

                // MİKTAR alanından al, yoksa 1 kullan
                decimal qty = isSearchMode ? 0 : 1;
                if (!isSearchMode && decimal.TryParse(txtQuantity.Text.Trim(), out decimal parsedQty) && parsedQty > 0)
                    qty = parsedQty;

                string barcode = input;

                // Miktar Çarpı Barkod Desteği (Örn: 5*869...) — geriye dönük uyumluluk
                if (input.Contains("*"))
                {
                    var parts = input.Split('*');
                    if (parts.Length == 2 && decimal.TryParse(parts[0], out decimal pQty) && pQty > 0)
                    {
                        qty = pQty;
                        barcode = parts[1];
                    }
                }

                ProcessBarcode(barcode, qty);
                txtBarcode.Clear();
                txtQuantity.Text = "1"; // Miktar alanını sıfırla
                
                if (!dgvCart.IsCurrentCellInEditMode)
                    txtBarcode.Focus();
            }
        }

        private void TxtQuantity_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                txtBarcode.Focus();
                txtBarcode.SelectAll();
            }
            // Sadece rakam, virgül/nokta ve silme tuşlarına izin ver
            bool isNum = (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) ||
                         (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9);
            bool isControl = e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete ||
                             e.KeyCode == Keys.Left || e.KeyCode == Keys.Right ||
                             e.KeyCode == (Keys)188 || e.KeyCode == Keys.OemPeriod ||
                             e.KeyCode == Keys.Decimal || e.Control;
            if (!isNum && !isControl && e.KeyCode != Keys.Enter)
                e.SuppressKeyPress = true;
        }

        private void ProcessBarcode(string barcode, decimal qty)
        {
            // Barkod okuyucudan gelen CR/LF ve boşlukları temizle
            barcode = barcode.Trim().Replace("\r", "").Replace("\n", "");
            if (string.IsNullOrEmpty(barcode)) return;

            // Terazi Barkodu Tespiti (27 ile başlayan 13 haneli)
            if (barcode.Length == 13 && barcode.StartsWith("27"))
            {
                string itemCode = barcode.Substring(2, 5);
                string weightStr = barcode.Substring(7, 5);
                if (decimal.TryParse(weightStr, out decimal weightKg))
                {
                    weightKg /= 1000m;
                    barcode = itemCode;
                    qty = weightKg;
                }
            }

            using (var db = new MarketDbContext())
            {
                // Dayanıklı arama — her iki taraftaki boşlukları eşleştir
                var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.Barcode.Trim() == barcode && p.IsActive);
                if (product != null)
                {
                    // GÖRSEL GÜNCELLEME
                    if (!string.IsNullOrEmpty(product.ImagePath) && System.IO.File.Exists(product.ImagePath))
                    {
                        try
                        {
                            using (var ms = new System.IO.MemoryStream(System.IO.File.ReadAllBytes(product.ImagePath)))
                            {
                                pbxProductPreview.Image = Image.FromStream(ms);
                            }
                        }
                        catch { pbxProductPreview.Image = null; }
                    }
                    else
                    {
                        pbxProductPreview.Image = null;
                    }

                    // BİLGİ GÜNCELLEME (MERKEZİ ALAN)
                    lblDetailName.Text = product.Name;
                    lblDetailPrice.Text = product.SalePrice.ToString("C2");
                    lblDetailCategory.Text = "Kategori: " + (product.Category?.Name ?? "Genel");

                    if (qty > 0)
                    {
                        var existing = cart.FirstOrDefault(x => x.ProductId == product.Id);
                        if (existing != null)
                        {
                            existing.Quantity += qty;
                            // Hızlı okuyucu için satırı güncelle yansıt:
                            int index = cart.IndexOf(existing);
                            cart.ResetItem(index);
                        }
                        else
                        {
                            cart.Add(new POSItem
                            {
                                ProductId = product.Id,
                                Barcode = product.Barcode,
                                ProductName = product.Name,
                                UnitPrice = product.SalePrice,
                                Quantity = qty,
                                CategoryName = product.Category?.Name ?? "Genel"
                            });
                        }
                        RefreshCart();
                        HardwareHelper.SendToPoleDisplay(product.Name, $"{product.SalePrice:C2}");
                    }
                }
                else
                {
                    pbxProductPreview.Image = null;
                    lblDetailName.Text = "Ürün bulunamadı!";
                    lblDetailPrice.Text = "";
                    lblDetailCategory.Text = "";
                    lblDetailName.ForeColor = Color.Red;
                    if (qty > 0)
                        MessageBox.Show("Ürün bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void RefreshCart()
        {
            if (dgvCart.DataSource == null)
            {
                dgvCart.DataSource = cart;
                if(dgvCart.Columns.Contains("ProductId")) dgvCart.Columns["ProductId"].Visible = false;
            }

            decimal araToplam = cart.Sum(x => x.SubTotal);
            // Kampanya ve İndirim Motoru
            decimal indirim = CampaignEngine.CalculateDiscount(cart.ToList(), out appliedCampaigns);

            decimal genelToplam = araToplam - indirim;
            if (genelToplam < 0) genelToplam = 0;

            lblTotal.Text = $"ARA TOPLAM: {araToplam:C2}";
            lblDiscount.Text = $"İNDİRİM: {indirim:C2}\n{appliedCampaigns}";
            lblGrandTotal.Text = $"GENEL TOPLAM:\n{genelToplam:C2}";
            
            // Kolon görünürlüğünü, başlıklarını ve düzenleme yetkisini ayala
            if (dgvCart.Columns.Count > 0)
            {
                foreach (DataGridViewColumn col in dgvCart.Columns) col.ReadOnly = true;

                if (dgvCart.Columns.Contains("ProductId")) dgvCart.Columns["ProductId"].Visible = false;
                if (dgvCart.Columns.Contains("Barcode")) dgvCart.Columns["Barcode"].HeaderText = "Barkod";
                if (dgvCart.Columns.Contains("ProductName")) dgvCart.Columns["ProductName"].HeaderText = "Ürün Adı";
                
                if (dgvCart.Columns.Contains("Quantity")) 
                {
                    dgvCart.Columns["Quantity"].HeaderText = "Miktar";
                    dgvCart.Columns["Quantity"].ReadOnly = false;
                    dgvCart.Columns["Quantity"].DefaultCellStyle.BackColor = Color.LemonChiffon; // Düzenlenebilir olduğunu belli et
                }
                
                if (dgvCart.Columns.Contains("UnitPrice")) 
                {
                    dgvCart.Columns["UnitPrice"].HeaderText = "Fiyat";
                    dgvCart.Columns["UnitPrice"].ReadOnly = false;
                    dgvCart.Columns["UnitPrice"].DefaultCellStyle.BackColor = Color.LemonChiffon;
                }

                if (dgvCart.Columns.Contains("SubTotal")) dgvCart.Columns["SubTotal"].HeaderText = "Tutar";
            }

            btnItemPlus.Enabled = cart.Any();
            btnItemMinus.Enabled = cart.Any();

            dgvCart.Refresh();
        }

        private void BtnCancelLine_Click(object? sender, EventArgs e)
        {
            DataGridViewRow? row = null;
            if (dgvCart.SelectedRows.Count > 0) row = dgvCart.SelectedRows[0];
            else if (dgvCart.CurrentRow != null) row = dgvCart.CurrentRow;

            if (row != null && row.Index >= 0)
            {
                int pId = Convert.ToInt32(row.Cells["ProductId"].Value);
                var item = cart.FirstOrDefault(x => x.ProductId == pId);
                if (item != null)
                {
                    cart.Remove(item);
                    Logger.LogActivity("POS_LINE_CANCEL", $"{item.ProductName} sepetten çıkarıldı.");
                    RefreshCart();
                }
            }
            txtBarcode.Focus();
        }

        // --- Miktar Düzenleme (Inline) ---

        private void DgvCart_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            var col = dgvCart.Columns[e.ColumnIndex];
            bool isQty = col.DataPropertyName == "Quantity" || col.Name == "Quantity";
            bool isPrice = col.DataPropertyName == "UnitPrice" || col.Name == "UnitPrice";

            if (!isQty && !isPrice)
            {
                e.Cancel = true;
                return;
            }

            // Düzenleme sırasında iptal butonlarını pasif yap
            btnCancelLine.Enabled = false;
            btnCancelAll.Enabled = false;
            btnItemPlus.Enabled = false;
            btnItemMinus.Enabled = false;
        }

        private void DgvCart_CellValidating(object? sender, DataGridViewCellValidatingEventArgs e)
        {
            var col = dgvCart.Columns[e.ColumnIndex];
            bool isQty = col.DataPropertyName == "Quantity" || col.Name == "Quantity";
            bool isPrice = col.DataPropertyName == "UnitPrice" || col.Name == "UnitPrice";

            if (!isQty && !isPrice) return;
            
            string input = e.FormattedValue?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(input) || !decimal.TryParse(input, out decimal val) || val <= 0)
            {
                e.Cancel = true;
                string fieldName = isQty ? "Miktar" : "Fiyat";
                MessageBox.Show($"{fieldName} boş veya 0 olamaz! Lütfen 0'dan büyük bir değer girin.", "Geçersiz Giriş", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DgvCart_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            var col = dgvCart.Columns[e.ColumnIndex];
            bool isQty = col.DataPropertyName == "Quantity" || col.Name == "Quantity";
            bool isPrice = col.DataPropertyName == "UnitPrice" || col.Name == "UnitPrice";

            if (!isQty && !isPrice) return;

            object? cellVal = dgvCart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (decimal.TryParse(cellVal?.ToString(), out decimal newVal) && newVal > 0)
            {
                int productIdColIdx = -1;
                foreach (DataGridViewColumn c in dgvCart.Columns) { if (c.DataPropertyName == "ProductId" || c.Name == "ProductId") { productIdColIdx = c.Index; break; } }
                if (productIdColIdx < 0) return;

                int pId = Convert.ToInt32(dgvCart.Rows[e.RowIndex].Cells[productIdColIdx].Value);
                var item = cart.FirstOrDefault(x => x.ProductId == pId);
                if (item != null)
                {
                    if (isQty) item.Quantity = newVal;
                    else if (isPrice) item.UnitPrice = newVal;

                    int index = cart.IndexOf(item);
                    cart.ResetItem(index); 
                    RefreshCart();
                }
            }
            else
            {
                RefreshCart();
            }

            // Düzenleme bitince butonları tekrar aktif et (RefreshCart zaten miktar butonlarını kontrol ediyor)
            btnCancelLine.Enabled = true;
            btnCancelAll.Enabled = true;
            
            txtBarcode.Focus();
        }

        private void BtnItemMinus_Click(object? sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count > 0)
            {
                int productIdColIdx = -1;
                foreach (DataGridViewColumn c in dgvCart.Columns) { if (c.DataPropertyName == "ProductId" || c.Name == "ProductId") { productIdColIdx = c.Index; break; } }
                if (productIdColIdx >= 0)
                {
                    int pId = Convert.ToInt32(dgvCart.SelectedRows[0].Cells[productIdColIdx].Value);
                    var item = cart.FirstOrDefault(x => x.ProductId == pId);
                    if (item != null && item.Quantity > 1)
                    {
                        item.Quantity -= 1;
                        int index = cart.IndexOf(item);
                        cart.ResetItem(index);
                        RefreshCart();
                    }
                }
            }
            txtBarcode.Focus();
        }

        private void BtnItemPlus_Click(object? sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count > 0)
            {
                int productIdColIdx = -1;
                foreach (DataGridViewColumn c in dgvCart.Columns) { if (c.DataPropertyName == "ProductId" || c.Name == "ProductId") { productIdColIdx = c.Index; break; } }
                if (productIdColIdx >= 0)
                {
                    int pId = Convert.ToInt32(dgvCart.SelectedRows[0].Cells[productIdColIdx].Value);
                    var item = cart.FirstOrDefault(x => x.ProductId == pId);
                    if (item != null)
                    {
                        item.Quantity += 1;
                        int index = cart.IndexOf(item);
                        cart.ResetItem(index);
                        RefreshCart();
                    }
                }
            }
            txtBarcode.Focus();
        }

        private void BtnCancelAll_Click(object? sender, EventArgs e)
        {
            if (cart.Any())
            {
                if (MessageBox.Show("Tüm fişi iptal etmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Logger.LogActivity("POS_ALL_CANCEL", $"Kasiyer fişi tamamen iptal etti. Tutar: {cart.Sum(x=>x.SubTotal)} TL");
                    cart.Clear();
                    selectedCustomer = null;
                    lblCustomer.Text = "Müşteri: STANDART";
                    txtBarcode.Clear();
                    txtQuantity.Text = "1";
                    RefreshCart();
                }
            }
            else
            {
                // Boş olsa bile Customer'ı resetle ve alanları temizle
                selectedCustomer = null;
                lblCustomer.Text = "Müşteri: STANDART";
                txtBarcode.Clear();
                txtQuantity.Text = "1";
                RefreshCart();
            }
            txtBarcode.Focus();
        }

        private void BtnSelectCustomer_Click(object? sender, EventArgs e)
        {
            using (var frm = new frmSelectCustomer())
            {
                if (frm.ShowDialog() == DialogResult.OK && frm.SelectedCustomer != null)
                {
                    selectedCustomer = frm.SelectedCustomer;
                    lblCustomer.Text = $"Müşteri: {selectedCustomer.FullName}\nPuan: {selectedCustomer.Points}\nBorç: {selectedCustomer.DebtBalance:C2}";
                }
            }
            txtBarcode.Focus();
        }

        private void BtnCash_Click(object? sender, EventArgs e)
        {
            if (!cart.Any()) return;
            decimal total = cart.Sum(x => x.SubTotal) - CampaignEngine.CalculateDiscount(cart.ToList(), out _);
            if (total <= 0) return;

            using (var kbd = new frmKeyboard("", isNumeric: true))
            {
                kbd.Text = "ALINAN NAKİT MİKTARI";
                if (kbd.ShowDialog() == DialogResult.OK)
                {
                    if (decimal.TryParse(kbd.InputText, out decimal alinan) && alinan >= total)
                    {
                        decimal paraUstu = alinan - total;
                        
                        // Kasa çekmecesini aç
                        HardwareHelper.OpenCashDrawer();
                        
                        string msg = $"TOPLAM: {total:C2}\nALINAN: {alinan:C2}\nPARA ÜSTÜ: {paraUstu:C2}\n\nKasa açıldı. Nakiti yerleştirip 'TAMAM' tuşuna basın.";
                        if (MessageBox.Show(msg, "NAKİT ÖDEME", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                        {
                            CompleteSale("Nakit");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Alınan miktar toplam tutardan az olamaz!", "Geçersiz Tutar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private async void BtnCard_Click(object? sender, EventArgs e)
        {
            if (!cart.Any()) return;
            decimal total = cart.Sum(x => x.SubTotal) - CampaignEngine.CalculateDiscount(cart.ToList(), out _);
            if (total <= 0) return;

            string selectedPort = cboComPorts.SelectedItem?.ToString() ?? "COM3";
            var posService = new HztsPosService(selectedPort);

            // Bekleme ekranı simüle et/göster
            btnCard.Enabled = false;
            string oldText = btnCard.Text;
            btnCard.Text = "POS BEKLENİYOR...";
            
            try 
            {
                var result = await posService.ProcessPaymentAsync(total);

                if (result.Success)
                {
                    CompleteSale("Kredi Kartı");
                }
                else
                {
                    MessageBox.Show(result.Message, "Ödeme Başarısız", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally 
            {
                btnCard.Enabled = true;
                btnCard.Text = oldText;
            }
        }

        private void BtnCredit_Click(object? sender, EventArgs e)
        {
            if (!cart.Any()) return;
            if (selectedCustomer == null)
            {
                MessageBox.Show("Veresiye satış (Borç Yazma) için önce listeden işlem yapılacak müşteriyi SEÇMELİSİNiZ!", "Müşteri Gerekli", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                BtnSelectCustomer_Click(null, EventArgs.Empty);
                if (selectedCustomer == null) return;
            }

            decimal araToplam = cart.Sum(x => x.SubTotal);
            decimal indirim = CampaignEngine.CalculateDiscount(cart.ToList(), out _);
            decimal genelToplam = araToplam - indirim;

            if (MessageBox.Show($"{selectedCustomer.FullName} adlı müşteriye {genelToplam:C2} tutarında Veresiye (Borç) kaydedilecektir.\nOnaylıyor musunuz?", "Veresiye İşlemi Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                CompleteSale("Veresiye");
            }
            txtBarcode.Focus();
        }

        private void CompleteSale(string paymentMethod)
        {
            if (!cart.Any()) return;

            decimal araToplam = cart.Sum(x => x.SubTotal);
            decimal indirim = CampaignEngine.CalculateDiscount(cart.ToList(), out _);
            decimal genelToplam = araToplam - indirim;

            using (var db = new MarketDbContext())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var sale = new Sale
                        {
                            ReceiptNumber = "R" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                            TotalAmount = araToplam,
                            DiscountAmount = indirim,
                            GrandTotal = genelToplam,
                            PaymentMethod = paymentMethod,
                            CashPaid = paymentMethod == "Nakit" ? genelToplam : 0,
                            CardPaid = paymentMethod == "Kredi Kartı" ? genelToplam : 0,
                            SaleDate = DateTime.Now,
                            UserId = ActiveUser.Id ?? 1,
                            CustomerId = selectedCustomer?.Id
                        };

                        db.Sales.Add(sale);
                        db.SaveChanges();

                        foreach (var item in cart)
                        {
                            var sd = new SaleDetail
                            {
                                SaleId = sale.Id,
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                UnitPrice = item.UnitPrice,
                                SubTotal = item.SubTotal
                            };
                            db.SaleDetails.Add(sd);

                            var p = db.Products.Find(item.ProductId);
                            if (p != null) p.StockQuantity -= item.Quantity;
                        }

                        if (selectedCustomer != null)
                        {
                            var c = db.Customers.Find(selectedCustomer.Id);
                            if (c != null)
                            {
                                c.Points += (int)(genelToplam / 100);
                                if (paymentMethod == "Veresiye")
                                {
                                    c.DebtBalance += genelToplam;
                                }
                            }
                        }

                        db.SaveChanges();
                        transaction.Commit();

                        Logger.LogActivity("SALE", $"{sale.ReceiptNumber} nolu satış tamamlandı. Tutar: {genelToplam:C2}");
                        
                        string receipt = $"FİŞ NO: {sale.ReceiptNumber}\nTARİH: {DateTime.Now}\nTOPLAM: {genelToplam:C2}\nÖDEME: {paymentMethod}\nTEŞEKKÜRLER!";
                        HardwareHelper.PrintReceiptAndOpenDrawer(receipt);
                        HardwareHelper.SendToPoleDisplay("TOPLAM TUTAR:", $"{genelToplam:C2}");
                        
                        MessageBox.Show($"Satış Başarılı!\nGenel Toplam: {genelToplam:C2}\nÖdeme Tipi: {paymentMethod}", "SATIŞ TAMAMLANDI", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        cart.Clear();
                        selectedCustomer = null;
                        lblCustomer.Text = "Müşteri: STANDART";
                        RefreshCart();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("Satış işlemi sırasında bir hata oluştu: " + ex.Message);
                    }
                }
            }
            txtBarcode.Focus();
        }

        private void DgvCart_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var col = dgvCart.Columns[e.ColumnIndex];
            bool isQty = col.DataPropertyName == "Quantity" || col.Name == "Quantity";
            bool isPrice = col.DataPropertyName == "UnitPrice" || col.Name == "UnitPrice";

            if (isQty || isPrice)
            {
                object? currentVal = dgvCart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                using (var kbd = new frmKeyboard(currentVal?.ToString() ?? "", isNumeric: true))
                {
                    if (kbd.ShowDialog() == DialogResult.OK)
                    {
                        if (decimal.TryParse(kbd.InputText, out decimal newVal) && newVal > 0)
                        {
                            dgvCart.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = kbd.InputText;
                            dgvCart.EndEdit();
                            RefreshCart();
                        }
                        else
                        {
                            MessageBox.Show("Miktar veya Fiyat 0'dan büyük olmalıdır!", "Geçersiz Değer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
        }

        private void DgvCart_EditingControlShowing(object? sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is TextBox txt)
            {
                txt.DoubleClick -= Control_VirtualKeyboard_DoubleClick;
                txt.DoubleClick += Control_VirtualKeyboard_DoubleClick;
            }
        }

        private void Control_VirtualKeyboard_DoubleClick(object? sender, EventArgs e)
        {
            if (sender is TextBox txt)
            {
                // Hücre Quantity veya UnitPrice mı kontrol et
                bool isNumericPad = dgvCart.CurrentCell != null && 
                                   (dgvCart.Columns[dgvCart.CurrentCell.ColumnIndex].DataPropertyName == "Quantity" || 
                                    dgvCart.Columns[dgvCart.CurrentCell.ColumnIndex].DataPropertyName == "UnitPrice");

                using (var kbd = new frmKeyboard(txt.Text, isNumeric: isNumericPad))
                {
                    if (kbd.ShowDialog() == DialogResult.OK)
                    {
                        if (isNumericPad && (!decimal.TryParse(kbd.InputText, out decimal newVal) || newVal <= 0))
                        {
                            MessageBox.Show("Miktar veya Fiyat 0'dan büyük olmalıdır!", "Geçersiz Değer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        txt.Text = kbd.InputText;
                        if (dgvCart.CurrentCell != null)
                        {
                            dgvCart.EndEdit();
                            RefreshCart();
                        }
                    }
                }
            }
        }

        private void DgvCart_CellEnter(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var col = dgvCart.Columns[e.ColumnIndex];
            bool isNumericCol = col.DataPropertyName == "Quantity" || col.Name == "Quantity" || 
                                col.DataPropertyName == "UnitPrice" || col.Name == "UnitPrice";

            if (isNumericCol)
            {
                btnCancelLine.Enabled = false;
                btnCancelAll.Enabled = false;
            }
            else
            {
                // Numerik olmayan sütunda ise sepet boş değilse aktif et
                btnCancelLine.Enabled = cart.Any();
                btnCancelAll.Enabled = cart.Any();
            }
        }
    }
}
