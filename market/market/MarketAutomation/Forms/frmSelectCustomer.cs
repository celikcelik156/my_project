using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Models;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmSelectCustomer : Form
    {
        public Customer? SelectedCustomer { get; private set; }
        private DataGridView dgvCustomers;
        private TextBox txtSearch;

        // Tahsilat Paneli
        private Panel pnlTahsilat;
        private TextBox txtTahsilatTutar;
        private int tahsilatMusteriId = 0;
        private string tahsilatMusteriAd = "";

        public frmSelectCustomer()
        {
            this.Size = new Size(980, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Müşteri Seçimi ve Cari (Borç) Tahsilatı";
            ThemeManager.ApplyTheme(this);

            var pnlTop = new Panel { Dock = DockStyle.Top, Height = 100, Padding = new Padding(20) };
            this.Controls.Add(pnlTop);

            pnlTop.Controls.Add(new Label { Text = "Müşteri Ara (Ad / Tel):", Location = new Point(20, 35), AutoSize = true });
            txtSearch = new TextBox { Location = new Point(230, 30), Width = 350, Font = new Font("Segoe UI", 16) };
            txtSearch.TextChanged += (s, e) => LoadData(txtSearch.Text);
            pnlTop.Controls.Add(txtSearch);

            Button btnSelect = new Button { Text = "SATIŞ İÇİN SEÇ", Location = new Point(600, 25), Width = 180, Height = 50, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnSelect.Click += BtnSelect_Click;
            pnlTop.Controls.Add(btnSelect);

            Button btnTahsilat = new Button { Text = "TAHSİLAT AL", Location = new Point(790, 25), Width = 170, Height = 50, BackColor = ThemeManager.SuccessColor, ForeColor = Color.White };
            btnTahsilat.Click += BtnTahsilat_Click;
            pnlTop.Controls.Add(btnTahsilat);

            dgvCustomers = new DataGridView
            {
                Location = new Point(0, 100),
                Size = new Size(980, 450), // Responsive on Anchor
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true,
                AllowUserToAddRows = false,
                RowTemplate = { Height = 45 },
                Font = new Font("Segoe UI", 14)
            };
            this.Controls.Add(dgvCustomers);

            // Tahsilat Paneli (Popup gibi)
            pnlTahsilat = new Panel 
            { 
                Visible = false, Size = new Size(400, 250), 
                Location = new Point(300, 200), 
                BackColor = Color.White, 
                BorderStyle = BorderStyle.FixedSingle 
            };
            
            Label lblTInfo = new Label { Text = "Nakit Tahsilat Tutarını Giriniz:", Location = new Point(30, 30), AutoSize = true, Font = ThemeManager.HeaderFont };
            txtTahsilatTutar = new TextBox { Location = new Point(30, 80), Width = 340, Font = new Font("Segoe UI", 24, FontStyle.Bold) };
            
            Button btnOnay = new Button { Text = "ONAYLA", Location = new Point(30, 150), Width = 160, Height = 60, BackColor = ThemeManager.SuccessColor, ForeColor = Color.White };
            btnOnay.Click += BtnTahsilatOnay_Click;
            
            Button btnIptal = new Button { Text = "İPTAL", Location = new Point(210, 150), Width = 160, Height = 60, BackColor = ThemeManager.DangerColor, ForeColor = Color.White };
            btnIptal.Click += (s, e) => { pnlTahsilat.Visible = false; txtSearch.Focus(); };

            pnlTahsilat.Controls.AddRange(new Control[] { lblTInfo, txtTahsilatTutar, btnOnay, btnIptal });
            this.Controls.Add(pnlTahsilat);
            pnlTahsilat.BringToFront();

            LoadData("");
        }

        private void LoadData(string search)
        {
            using (var db = new MarketDbContext())
            {
                var query = db.Customers.AsQueryable();
                if (!string.IsNullOrWhiteSpace(search))
                    query = query.Where(c => c.FullName.ToLower().Contains(search.ToLower()) || c.Phone.Contains(search));

                var list = query.ToList();
                dgvCustomers.DataSource = list.Select(c => new {
                    c.Id,
                    MusteriAdi = c.FullName,
                    Telefon = c.Phone,
                    GuncelDurum = c.DebtBalance > 0 ? "[- BORÇLU]" : (c.DebtBalance < 0 ? "[+ ALACAKLI]" : "[TEMİZ]"),
                    Tutar_TL = Math.Round(Math.Abs(c.DebtBalance), 2),
                    Puan = c.Points
                }).ToList();
            }
        }

        private void BtnSelect_Click(object? sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                int cId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["Id"].Value);
                using (var db = new MarketDbContext())
                {
                    SelectedCustomer = db.Customers.Find(cId);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else { MessageBox.Show("Lütfen listeden bir müşteri seçin.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void BtnTahsilat_Click(object? sender, EventArgs e)
        {
            if (dgvCustomers.SelectedRows.Count > 0)
            {
                tahsilatMusteriId = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["Id"].Value);
                tahsilatMusteriAd = dgvCustomers.SelectedRows[0].Cells["MusteriAdi"].Value?.ToString() ?? "";
                
                txtTahsilatTutar.Clear();
                pnlTahsilat.Visible = true;
                txtTahsilatTutar.Focus();
                pnlTahsilat.BringToFront();
            }
            else { MessageBox.Show("Tahsilat girmek için listeden borçlu müşteriyi seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void BtnTahsilatOnay_Click(object? sender, EventArgs e)
        {
            if (decimal.TryParse(txtTahsilatTutar.Text, out decimal amount) && amount > 0)
            {
                using (var db = new MarketDbContext())
                {
                    var c = db.Customers.Find(tahsilatMusteriId);
                    if (c != null)
                    {
                        c.DebtBalance -= amount; // Borç eksilir
                        
                        // Kasa Nakit Akışı İçin Satış Tablosuna Tahsilat İşlemi "Ekleme" (+ Nakit)
                        var tahsilatKaydi = new Sale
                        {
                            ReceiptNumber = "T" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                            TotalAmount = 0,
                            DiscountAmount = 0,
                            GrandTotal = 0,
                            PaymentMethod = "Tahsilat",
                            CashPaid = amount,
                            CardPaid = 0,
                            SaleDate = DateTime.Now,
                            UserId = ActiveUser.Id ?? 1,
                            CustomerId = c.Id
                        };
                        db.Sales.Add(tahsilatKaydi);

                        Logger.LogActivity("TAHSILAT", $"{tahsilatMusteriAd} kişisinden {amount:C2} nakit tahsilat yapıldı. Kalan Borç: {c.DebtBalance:C2}");
                        db.SaveChanges();
                        MessageBox.Show($"Tahsilat İşlemi Başarılı!\nKasa'ya Nakit Girdi (+)\nMüşteri Borcu Eksildi (-)\n\nMüşteri: {tahsilatMusteriAd}\nAlınan Tutar: {amount:C2}\nKalan Borç Durumu: {(c.DebtBalance > 0 ? c.DebtBalance + " TL BORÇLU" : Math.Abs(c.DebtBalance) + " TL ALACAKLI")}", "Tahsilat (Ödeme) Onaylandı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        pnlTahsilat.Visible = false;
                        LoadData(txtSearch.Text);
                    }
                }
            }
            else { MessageBox.Show("Hatalı tutar girdiniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}
