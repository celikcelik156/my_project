using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Models;
using MarketAutomation.Helpers;
using ClosedXML.Excel;

namespace MarketAutomation.Forms
{
    public class frmCustomer : Form
    {
        private DataGridView dgvCustomers;
        private TextBox txtFullName, txtPhone, txtAddress, txtCollectionAmount;
        private Button btnSave, btnDelete, btnCollect, btnSendSms, btnClose;
        private int selectedId = 0;

        public frmCustomer()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(1100, 700);
            this.Text = "Müşteri ve Veresiye Yönetimi";

            var panelLeft = new Panel { Dock = DockStyle.Left, Width = 450, Padding = new Padding(20), AutoScroll = true };
            this.Controls.Add(panelLeft);

            var lblTitle = new Label { Text = "MÜŞTERİ BİLGİLERİ", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, 20) };
            panelLeft.Controls.Add(lblTitle);

            int yPos = 70;
            panelLeft.Controls.Add(new Label { Text = "Ad Soyad:", AutoSize = true, Location = new Point(20, yPos) });
            txtFullName = new TextBox { Location = new Point(20, yPos + 40), Width = 300 };
            panelLeft.Controls.Add(txtFullName);
            yPos += 90;

            panelLeft.Controls.Add(new Label { Text = "Telefon:", AutoSize = true, Location = new Point(20, yPos) });
            txtPhone = new TextBox { Location = new Point(20, yPos + 40), Width = 300 };
            panelLeft.Controls.Add(txtPhone);
            yPos += 90;

            panelLeft.Controls.Add(new Label { Text = "Adres:", AutoSize = true, Location = new Point(20, yPos) });
            txtAddress = new TextBox { Location = new Point(20, yPos + 40), Width = 300, Multiline = true, Height = 60 };
            panelLeft.Controls.Add(txtAddress);
            yPos += 130;

            btnSave = new Button { Text = "KAYDET", Location = new Point(20, yPos), Width = 140, Height = 45, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnSave.Click += BtnSave_Click;
            btnDelete = new Button { Text = "SİL", Location = new Point(180, yPos), Width = 140, Height = 45, BackColor = ThemeManager.DangerColor, ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;
            panelLeft.Controls.AddRange(new Control[] { btnSave, btnDelete });
            yPos += 70;

            // Tahsilat Bölümü
            var lblTahsilat = new Label { Text = "TAHSİLAT YAP", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, yPos), ForeColor = ThemeManager.SuccessColor };
            panelLeft.Controls.Add(lblTahsilat);
            yPos += 40;

            panelLeft.Controls.Add(new Label { Text = "Tutar (TL):", AutoSize = true, Location = new Point(20, yPos + 5) });
            txtCollectionAmount = new TextBox { Location = new Point(140, yPos), Width = 100, Font = new Font("Segoe UI", 12, FontStyle.Bold) };
            panelLeft.Controls.Add(txtCollectionAmount);

            btnCollect = new Button { Text = "ÖDEME AL", Location = new Point(260, yPos - 5), Width = 150, Height = 35, BackColor = ThemeManager.SuccessColor, ForeColor = Color.White };
            btnCollect.Click += BtnCollect_Click;
            panelLeft.Controls.Add(btnCollect);
            yPos += 50;

            btnSendSms = new Button { Text = "BORÇ HATIRLATMA SMS'İ GÖNDER", Location = new Point(20, yPos), Width = 300, Height = 45, BackColor = Color.Orange, ForeColor = Color.White };
            btnSendSms.Click += BtnSendSms_Click;
            panelLeft.Controls.Add(btnSendSms);
            yPos += 60;

            btnClose = new Button { Text = "KAPAT", Location = new Point(20, yPos), Width = 300, Height = 45, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();
            yPos += 55;

            var btnExport = new Button { Text = "EXCEL'E AKTAR (Yedek Al)", Location = new Point(20, yPos), Width = 380, Height = 45, BackColor = Color.FromArgb(33, 115, 70), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnExport.Click += BtnExport_Click;

            var btnImport = new Button { Text = "EXCEL'DEN YÜKLE (Güncelle)", Location = new Point(20, yPos + 55), Width = 380, Height = 45, BackColor = Color.DarkOrange, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnImport.Click += BtnImport_Click;

            panelLeft.Controls.AddRange(new Control[] { btnClose, btnExport, btnImport });

            dgvCustomers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = ThemeManager.BackgroundColor
            };
            dgvCustomers.CellDoubleClick += Dgv_CellDoubleClick;
            this.Controls.Add(dgvCustomers);
            dgvCustomers.BringToFront();
        }

        private void LoadData()
        {
            using (var db = new MarketDbContext())
            {
                dgvCustomers.DataSource = db.Customers.Select(c => new { 
                    c.Id, 
                    AdSoyad = c.FullName, 
                    Telefon = c.Phone, 
                    Puan = c.Points, 
                    Borc_TL = c.DebtBalance 
                }).ToList();
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text)) return;

            using (var db = new MarketDbContext())
            {
                if (selectedId == 0)
                {
                    db.Customers.Add(new Customer 
                    { 
                        FullName = txtFullName.Text, 
                        Phone = txtPhone.Text, 
                        Address = txtAddress.Text 
                    });
                }
                else
                {
                    var c = db.Customers.Find(selectedId);
                    if (c != null)
                    {
                        c.FullName = txtFullName.Text;
                        c.Phone = txtPhone.Text;
                        c.Address = txtAddress.Text;
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
            if (MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var db = new MarketDbContext())
                {
                    var c = db.Customers.Find(selectedId);
                    if (c != null)
                    {
                        db.Customers.Remove(c);
                        db.SaveChanges();
                    }
                }
                ClearForm();
                LoadData();
            }
        }

        private void BtnCollect_Click(object? sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Önce listeden müşteri seçin."); return; }
            if (decimal.TryParse(txtCollectionAmount.Text, out decimal amount) && amount > 0)
            {
                using (var db = new MarketDbContext())
                {
                    var c = db.Customers.Find(selectedId);
                    if (c != null)
                    {
                        c.DebtBalance -= amount;
                        db.SaveChanges();
                        Logger.LogActivity("DEBT_COLLECTION", $"{c.FullName} adlı müşteriden {amount:C2} tahsilat yapıldı. Kalan Borç: {c.DebtBalance:C2}");
                        MessageBox.Show("Tahsilat başarıyla işlendi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                txtCollectionAmount.Clear();
                LoadData();
            }
        }

        private void BtnSendSms_Click(object? sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Önce listeden müşteri seçin."); return; }
            using (var db = new MarketDbContext())
            {
                var c = db.Customers.Find(selectedId);
                if (c != null && c.DebtBalance > 0 && !string.IsNullOrWhiteSpace(c.Phone))
                {
                    string message = $"Sayın {c.FullName}, marketimize olan borcunuz {c.DebtBalance:C2} tutarındadır. Lütfen en kısa sürede ödeme yapınız. İyi günler dileriz.";
                    bool success = SmsHelper.SendSms(c.Phone, message);
                    if (success)
                    {
                        MessageBox.Show("SMS Başarıyla Gönderildi!", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Logger.LogActivity("SMS_SENT", $"{c.Phone} numarasına borç hatırlatması atıldı.");
                    }
                }
                else
                {
                    MessageBox.Show("Müşterinin borcu yok veya geçerli bir telefon numarası bulunmuyor!");
                }
            }
        }

        private void Dgv_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = Convert.ToInt32(dgvCustomers.Rows[e.RowIndex].Cells["Id"].Value);
                using (var db = new MarketDbContext())
                {
                    var c = db.Customers.Find(selectedId);
                    if (c != null)
                    {
                        txtFullName.Text = c.FullName;
                        txtPhone.Text = c.Phone;
                        txtAddress.Text = c.Address;
                    }
                }
            }
        }

        private void ClearForm()
        {
            selectedId = 0;
            txtFullName.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Çalışma Kitabı|*.xlsx", FileName = "Musteriler_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Müşteriler");
                            ws.Cell(1, 1).Value = "Ad Soyad";
                            ws.Cell(1, 2).Value = "Telefon";
                            ws.Cell(1, 3).Value = "Adres";
                            ws.Cell(1, 4).Value = "Puan";
                            ws.Cell(1, 5).Value = "Güncel Borç (TL)";

                            var header = ws.Range(1, 1, 1, 5);
                            header.Style.Font.Bold = true;
                            header.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                            header.Style.Font.FontColor = XLColor.White;

                            using (var db = new MarketDbContext())
                            {
                                var customers = db.Customers.ToList();
                                int row = 2;
                                foreach (var c in customers)
                                {
                                    ws.Cell(row, 1).Value = c.FullName;
                                    ws.Cell(row, 2).Value = c.Phone;
                                    ws.Cell(row, 3).Value = c.Address;
                                    ws.Cell(row, 4).Value = c.Points;
                                    ws.Cell(row, 5).Value = c.DebtBalance;
                                    row++;
                                }
                            }
                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            MessageBox.Show("Müşteriler başarıyla Excel'e aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (MessageBox.Show("Ad Soyad eşleşen müşterilerin bilgileri güncellenecek, eşleşmeyenler yeni olarak eklenecektir.\nDevam edilsin mi?", "Toplu Güncelleme Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
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
                            var rows = ws.RangeUsed().RowsUsed().Skip(1);
                            
                            int updatedCount = 0;
                            int addedCount = 0;

                            foreach (var row in rows)
                            {
                                string fullName = row.Cell(1).Value.ToString().Trim();
                                if (string.IsNullOrEmpty(fullName)) continue;

                                string phone = row.Cell(2).Value.ToString().Trim();
                                string address = row.Cell(3).Value.ToString().Trim();
                                int.TryParse(row.Cell(4).Value.ToString(), out int points);
                                decimal.TryParse(row.Cell(5).Value.ToString(), out decimal debt);

                                var existing = db.Customers.FirstOrDefault(c => c.FullName == fullName);
                                if (existing != null)
                                {
                                    existing.Phone = phone;
                                    existing.Address = address;
                                    existing.Points = points;
                                    existing.DebtBalance = debt;
                                    updatedCount++;
                                }
                                else
                                {
                                    db.Customers.Add(new Customer 
                                    {
                                        FullName = fullName,
                                        Phone = phone,
                                        Address = address,
                                        Points = points,
                                        DebtBalance = debt
                                    });
                                    addedCount++;
                                }
                            }

                            db.SaveChanges();
                            Logger.LogActivity("EXCEL_SYNC", $"Müşteri Excel senk: {updatedCount} güncellendi, {addedCount} eklendi.");
                            MessageBox.Show($"Senkronizasyon Tamamlandı!\nGüncellenen: {updatedCount}\nYeni Eklenen: {addedCount}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
