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
    public class frmSupplier : Form
    {
        private DataGridView dgvSuppliers;
        private TextBox txtCompanyName, txtContact, txtPhone, txtAddress;
        private Button btnSave, btnDelete, btnClose;
        private int selectedId = 0;

        public frmSupplier()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(950, 650);
            this.Text = "Tedarikçi (Toptancı) Yönetimi";

            var panelLeft = new Panel { Dock = DockStyle.Left, Width = 450, Padding = new Padding(20), AutoScroll = true };
            this.Controls.Add(panelLeft);

            var lblTitle = new Label { Text = "TEDARİKÇİ BİLGİLERİ", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, 20) };
            panelLeft.Controls.Add(lblTitle);

            // Fields
            string[] labels = { "Firma Adı:", "Yetkili Kişi:", "Telefon:", "Adres:" };
            TextBox[] textboxes = new TextBox[4];

            for (int i = 0; i < 4; i++)
            {
                panelLeft.Controls.Add(new Label { Text = labels[i], AutoSize = true, Location = new Point(20, 70 + (i * 100)) });
                textboxes[i] = new TextBox { Location = new Point(20, 110 + (i * 100)), Width = 300, Font = ThemeManager.PrimaryFont };
                if (i == 3) { textboxes[i].Multiline = true; textboxes[i].Height = 60; }
                panelLeft.Controls.Add(textboxes[i]);
            }

            txtCompanyName = textboxes[0];
            txtContact = textboxes[1];
            txtPhone = textboxes[2];
            txtAddress = textboxes[3];

            btnSave = new Button { Text = "KAYDET", Location = new Point(20, 480), Width = 140, Height = 45, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnSave.Click += BtnSave_Click;

            btnDelete = new Button { Text = "SİL", Location = new Point(200, 480), Width = 140, Height = 45, BackColor = ThemeManager.DangerColor, ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;

            btnClose = new Button { Text = "KAPAT", Location = new Point(20, 540), Width = 320, Height = 45, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();

            var btnExport = new Button { Text = "EXCEL'E AKTAR (Yedek Al)", Location = new Point(20, 600), Width = 380, Height = 45, BackColor = Color.FromArgb(33, 115, 70), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnExport.Click += BtnExport_Click;

            var btnImport = new Button { Text = "EXCEL'DEN YÜKLE (Güncelle)", Location = new Point(20, 655), Width = 380, Height = 45, BackColor = Color.DarkOrange, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnImport.Click += BtnImport_Click;

            panelLeft.Controls.AddRange(new Control[] { btnSave, btnDelete, btnClose, btnExport, btnImport });

            dgvSuppliers = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = ThemeManager.BackgroundColor
            };
            dgvSuppliers.CellDoubleClick += Dgv_CellDoubleClick;
            this.Controls.Add(dgvSuppliers);
            dgvSuppliers.BringToFront();
        }

        private void LoadData()
        {
            using (var db = new MarketDbContext())
            {
                dgvSuppliers.DataSource = db.Suppliers.Select(s => new { s.Id, s.CompanyName, s.ContactName, s.Phone, s.Balance }).ToList();
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text)) return;

            using (var db = new MarketDbContext())
            {
                if (selectedId == 0)
                {
                    db.Suppliers.Add(new Supplier 
                    { 
                        CompanyName = txtCompanyName.Text, 
                        ContactName = txtContact.Text, 
                        Phone = txtPhone.Text, 
                        Address = txtAddress.Text 
                    });
                    Logger.LogActivity("SUPPLIER_ADD", $"{txtCompanyName.Text} eklendi.");
                }
                else
                {
                    var sup = db.Suppliers.Find(selectedId);
                    if (sup != null)
                    {
                        sup.CompanyName = txtCompanyName.Text;
                        sup.ContactName = txtContact.Text;
                        sup.Phone = txtPhone.Text;
                        sup.Address = txtAddress.Text;
                        Logger.LogActivity("SUPPLIER_UPDATE", $"{txtCompanyName.Text} güncellendi.");
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
                    var sup = db.Suppliers.Find(selectedId);
                    if (sup != null)
                    {
                        db.Suppliers.Remove(sup);
                        db.SaveChanges();
                        Logger.LogActivity("SUPPLIER_DELETE", $"{sup.CompanyName} silindi.");
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
                selectedId = Convert.ToInt32(dgvSuppliers.Rows[e.RowIndex].Cells["Id"].Value);
                using (var db = new MarketDbContext())
                {
                    var sup = db.Suppliers.Find(selectedId);
                    if (sup != null)
                    {
                        txtCompanyName.Text = sup.CompanyName;
                        txtContact.Text = sup.ContactName;
                        txtPhone.Text = sup.Phone;
                        txtAddress.Text = sup.Address;
                    }
                }
            }
        }

        private void ClearForm()
        {
            selectedId = 0;
            txtCompanyName.Clear();
            txtContact.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Çalışma Kitabı|*.xlsx", FileName = "Tedarikciler_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Tedarikçiler");
                            ws.Cell(1, 1).Value = "Firma Adı";
                            ws.Cell(1, 2).Value = "Yetkili Kişi";
                            ws.Cell(1, 3).Value = "Telefon";
                            ws.Cell(1, 4).Value = "Adres";
                            ws.Cell(1, 5).Value = "Bakiye";

                            var header = ws.Range(1, 1, 1, 5);
                            header.Style.Font.Bold = true;
                            header.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                            header.Style.Font.FontColor = XLColor.White;

                            using (var db = new MarketDbContext())
                            {
                                var suppliers = db.Suppliers.ToList();
                                int row = 2;
                                foreach (var s in suppliers)
                                {
                                    ws.Cell(row, 1).Value = s.CompanyName;
                                    ws.Cell(row, 2).Value = s.ContactName;
                                    ws.Cell(row, 3).Value = s.Phone;
                                    ws.Cell(row, 4).Value = s.Address;
                                    ws.Cell(row, 5).Value = s.Balance;
                                    row++;
                                }
                            }
                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            MessageBox.Show("Tedarikçiler başarıyla Excel'e aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (MessageBox.Show("Firma Adı eşleşen tedarikçilerin bilgileri güncellenecek, eşleşmeyenler yeni olarak eklenecektir.\nDevam edilsin mi?", "Toplu Güncelleme Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
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
                                string compName = row.Cell(1).Value.ToString().Trim();
                                if (string.IsNullOrEmpty(compName)) continue;

                                string contact = row.Cell(2).Value.ToString().Trim();
                                string phone = row.Cell(3).Value.ToString().Trim();
                                string address = row.Cell(4).Value.ToString().Trim();
                                decimal.TryParse(row.Cell(5).Value.ToString(), out decimal balance);

                                var existing = db.Suppliers.FirstOrDefault(s => s.CompanyName == compName);
                                if (existing != null)
                                {
                                    existing.ContactName = contact;
                                    existing.Phone = phone;
                                    existing.Address = address;
                                    existing.Balance = balance;
                                    updatedCount++;
                                }
                                else
                                {
                                    db.Suppliers.Add(new Supplier 
                                    {
                                        CompanyName = compName,
                                        ContactName = contact,
                                        Phone = phone,
                                        Address = address,
                                        Balance = balance
                                    });
                                    addedCount++;
                                }
                            }

                            db.SaveChanges();
                            Logger.LogActivity("EXCEL_SYNC", $"Tedarikçi Excel senk: {updatedCount} güncellendi, {addedCount} eklendi.");
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
