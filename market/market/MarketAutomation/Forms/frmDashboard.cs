using System;
using System.Drawing;
using System.Windows.Forms;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmDashboard : Form
    {
        private Label lblWelcome;
        private Button btnLogout, btnPOS, btnProducts, btnCategories, btnSuppliers, btnPurchaseInvoice, btnInventory, btnWarnings, btnCustomers, btnReports;
        private Label lblRole;
        private Panel panelSide, panelTop, panelMain;
        private FlowLayoutPanel pnlTiles;

        public frmDashboard()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            ApplyCustomTheme();
        }

        private void InitializeComponent()
        {
            this.lblWelcome = new Label();
            this.lblRole = new Label();
            this.btnLogout = new Button();
            this.btnPOS = new Button();
            this.btnProducts = new Button();
            this.btnCategories = new Button();
            this.btnSuppliers = new Button();
            this.btnPurchaseInvoice = new Button();
            this.btnInventory = new Button();
            this.btnWarnings = new Button();
            this.btnCustomers = new Button();
            this.btnReports = new Button();
            
            this.panelSide = new Panel();
            this.panelTop = new Panel();
            this.panelMain = new Panel();

            this.SuspendLayout();

            this.ClientSize = new Size(1200, 800);
            this.Text = "Ana Menü - Market Otomasyonu";
            this.WindowState = FormWindowState.Maximized;

            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 100;
            panelTop.BackColor = ThemeManager.SurfaceColor;
            this.Controls.Add(panelTop);

            this.lblWelcome.Location = new Point(30, 20);
            this.lblWelcome.AutoSize = true;
            panelTop.Controls.Add(this.lblWelcome);

            this.lblRole.Location = new Point(30, 60);
            this.lblRole.AutoSize = true;
            panelTop.Controls.Add(this.lblRole);

            panelSide.Dock = DockStyle.Left;
            panelSide.Width = 300;
            panelSide.BackColor = ThemeManager.SecondaryColor;
            panelSide.AutoScroll = true;
            this.Controls.Add(panelSide);

            panelMain.Dock = DockStyle.Fill;
            panelMain.BackColor = ThemeManager.BackgroundColor;
            this.Controls.Add(panelMain);

            pnlTiles = new FlowLayoutPanel
            {
                Dock = DockStyle.None,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(20),
                BackColor = Color.Transparent
            };
            panelMain.Controls.Add(pnlTiles);

            int btnY = 30;
            
            btnPOS.Text = "HIZLI SATIŞ (POS)";
            btnPOS.Location = new Point(20, btnY);
            btnPOS.Size = new Size(260, 60);
            btnPOS.Click += (s, e) => new frmPOS().ShowDialog();
            panelSide.Controls.Add(btnPOS);
            btnY += 80;

            btnProducts.Text = "ÜRÜN YÖNETİMİ";
            btnProducts.Location = new Point(20, btnY);
            btnProducts.Size = new Size(260, 40);
            btnProducts.Click += (s, e) => new frmProduct().ShowDialog();
            panelSide.Controls.Add(btnProducts);
            btnY += 50;

            btnCategories.Text = "KATEGORİ YÖNETİMİ";
            btnCategories.Location = new Point(20, btnY);
            btnCategories.Size = new Size(260, 40);
            btnCategories.Click += (s, e) => new frmCategory().ShowDialog();
            panelSide.Controls.Add(btnCategories);
            btnY += 50;

            btnCustomers.Text = "MÜŞTERİ V. YÖNETİMİ";
            btnCustomers.Location = new Point(20, btnY);
            btnCustomers.Size = new Size(260, 40);
            btnCustomers.Click += (s, e) => new frmCustomer().ShowDialog();
            panelSide.Controls.Add(btnCustomers);
            btnY += 50;

            btnSuppliers.Text = "TEDARİKÇİ YÖNETİMİ";
            btnSuppliers.Location = new Point(20, btnY);
            btnSuppliers.Size = new Size(260, 40);
            btnSuppliers.Click += (s, e) => new frmSupplier().ShowDialog();
            panelSide.Controls.Add(btnSuppliers);
            btnY += 50;

            btnPurchaseInvoice.Text = "ALIŞ FATURASI GİRİŞİ";
            btnPurchaseInvoice.Location = new Point(20, btnY);
            btnPurchaseInvoice.Size = new Size(260, 40);
            btnPurchaseInvoice.Click += (s, e) => new frmPurchaseInvoice().ShowDialog();
            panelSide.Controls.Add(btnPurchaseInvoice);
            btnY += 50;

            btnInventory.Text = "ENVANTER SAYIMI";
            btnInventory.Location = new Point(20, btnY);
            btnInventory.Size = new Size(260, 40);
            btnInventory.Click += (s, e) => new frmInventoryCount().ShowDialog();
            panelSide.Controls.Add(btnInventory);
            btnY += 50;

            btnWarnings.Text = "SKT VE KRİTİK STOK UYARILARI";
            btnWarnings.Location = new Point(20, btnY);
            btnWarnings.Size = new Size(260, 40);
            btnWarnings.Click += (s, e) => new frmExpiryWarnings().ShowDialog();
            panelSide.Controls.Add(btnWarnings);
            btnY += 50;

            btnReports.Text = "Z RAPORU VE ANALİZ";
            btnReports.Location = new Point(20, btnY);
            btnReports.Size = new Size(260, 60);
            btnReports.Click += (s, e) => new frmReports().ShowDialog();
            panelSide.Controls.Add(btnReports);
            btnY += 80;

            this.btnLogout.Text = "ÇIKIŞ YAP";
            this.btnLogout.Location = new Point(20, btnY);
            this.btnLogout.Size = new Size(260, 50);
            this.btnLogout.Click += BtnLogout_Click;
            panelSide.Controls.Add(this.btnLogout);

            this.ResumeLayout(false);
            this.Load += FrmDashboard_Load;
        }

        private void FrmDashboard_Load(object? sender, EventArgs e)
        {
            this.lblWelcome.Text = $"Hoşgeldin, {ActiveUser.FullName}";
            this.lblRole.Text = $"Yetki: {ActiveUser.Role}";
            
            if (ActiveUser.IsAdmin)
            {
                // Admin için Yan Paneli Gizle ve Orta Paneli Dev Panellerle Doldur
                panelSide.Visible = false;
                SetupAdminTiles();
            }
            else
            {
                // Kasiyer için sadece POS açık kalsın
                btnProducts.Enabled = false;
                btnCategories.Enabled = false;
                btnSuppliers.Enabled = false;
                btnPurchaseInvoice.Enabled = false;
                btnInventory.Enabled = false;
                btnWarnings.Enabled = false;
                btnCustomers.Enabled = false;
                btnReports.Enabled = false;
            }
        }

        private void SetupAdminTiles()
        {
            pnlTiles.Controls.Clear();
            pnlTiles.WrapContents = true;
            // 4 veya 5 buton yan yana gelecek şekilde genişlik sınırı koy (220+20 margin * 4 = 960)
            pnlTiles.MaximumSize = new Size(1000, 0); 
            
            AddTile("HIZLI SATIŞ (POS)", "🛒", Color.FromArgb(46, 204, 113), () => new frmPOS().ShowDialog());
            AddTile("ÜRÜN YÖNETİMİ", "📦", Color.FromArgb(52, 152, 219), () => new frmProduct().ShowDialog());
            AddTile("KATEGORİLER", "🏷️", Color.FromArgb(155, 89, 182), () => new frmCategory().ShowDialog());
            AddTile("MÜŞTERİ / VERESİYE", "👥", Color.FromArgb(241, 196, 15), () => new frmCustomer().ShowDialog());
            AddTile("TEDARİKÇİLER", "🚚", Color.FromArgb(230, 126, 34), () => new frmSupplier().ShowDialog());
            AddTile("ALIŞ FATURASI", "📄", Color.FromArgb(52, 73, 94), () => new frmPurchaseInvoice().ShowDialog());
            AddTile("ENVANTER SAYIMI", "📊", Color.FromArgb(22, 160, 133), () => new frmInventoryCount().ShowDialog());
            AddTile("ANALİZ VE RAPOR", "📈", Color.FromArgb(192, 57, 43), () => new frmReports().ShowDialog());
            AddTile("ÇIKIŞ YAP", "🚪", Color.FromArgb(149, 165, 166), () => BtnLogout_Click(null, null));

            // İlk yerleşim
            CenterTiles();
            
            panelMain.Resize += (s, e) => CenterTiles();
        }

        private void CenterTiles()
        {
            if (pnlTiles != null && panelMain != null)
            {
                pnlTiles.Location = new Point((panelMain.Width - pnlTiles.Width) / 2, 
                                              (panelMain.Height - pnlTiles.Height) / 2);
            }
        }

        private void AddTile(string text, string emoji, Color color, Action onClick)
        {
            Button btn = new Button
            {
                Size = new Size(220, 220),
                Text = $"{emoji}\n\n{text}",
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(10)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            pnlTiles.Controls.Add(btn);
        }

        private void ApplyCustomTheme()
        {
            this.lblWelcome.Font = ThemeManager.HeaderFont;
            this.btnPOS.BackColor = ThemeManager.SuccessColor;
            
            this.btnProducts.BackColor = ThemeManager.PrimaryColor;
            this.btnCategories.BackColor = ThemeManager.PrimaryColor;
            this.btnCustomers.BackColor = ThemeManager.PrimaryColor;
            this.btnSuppliers.BackColor = ThemeManager.PrimaryColor;
            this.btnPurchaseInvoice.BackColor = ThemeManager.PrimaryColor;
            this.btnInventory.BackColor = ThemeManager.PrimaryColor;
            
            this.btnWarnings.BackColor = ThemeManager.DangerColor;
            this.btnWarnings.ForeColor = ThemeManager.PrimaryTextColor;

            this.btnReports.BackColor = ThemeManager.DangerColor; // Göze batsın
            this.btnReports.ForeColor = ThemeManager.PrimaryTextColor;

            this.btnLogout.BackColor = ThemeManager.SecondaryColor;
            this.btnLogout.ForeColor = ThemeManager.PrimaryTextColor;
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            Logger.LogActivity("LOGOUT", $"{ActiveUser.Username} sistemden çıkış yaptı.");
            BackupHelper.AutoBackupDatabase();
            Application.Restart();
        }
    }
}
