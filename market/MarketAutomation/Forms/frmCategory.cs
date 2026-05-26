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
    public class frmCategory : Form
    {
        private DataGridView dgvCategories;
        private TextBox txtName, txtDescription;
        private Button btnSave, btnDelete, btnClose;
        private int selectedCategoryId = 0;

        public frmCategory()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            LoadCategories();
        }

        private void InitializeComponent()
        {
            this.Size = new Size(800, 600);
            this.Text = "Kategori Yönetimi";

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = ThemeManager.SurfaceColor };
            this.Controls.Add(panelTop);

            var lblTitle = new Label { Text = "KATEGORİ İŞLEMLERİ", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, 20) };
            panelTop.Controls.Add(lblTitle);

            // Inputs
            var panelLeft = new Panel { Dock = DockStyle.Left, Width = 450, Padding = new Padding(20), AutoScroll = true };
            this.Controls.Add(panelLeft);

            var lblName = new Label { Text = "Kategori Adı:", AutoSize = true, Location = new Point(20, 20) };
            txtName = new TextBox { Location = new Point(20, 60), Width = 300 };
            
            var lblDesc = new Label { Text = "Açıklama:", AutoSize = true, Location = new Point(20, 110) };
            txtDescription = new TextBox { Location = new Point(20, 150), Width = 300, Multiline = true, Height = 80 };

            btnSave = new Button { Text = "KAYDET", Location = new Point(20, 270), Width = 140, Height = 40, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnSave.Click += BtnSave_Click;

            btnDelete = new Button { Text = "SİL", Location = new Point(180, 270), Width = 140, Height = 40, BackColor = ThemeManager.DangerColor, ForeColor = Color.White };
            btnDelete.Click += BtnDelete_Click;

            btnClose = new Button { Text = "KAPAT", Location = new Point(20, 330), Width = 300, Height = 40, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();

            var btnExport = new Button { Text = "EXCEL'E AKTAR (Yedek Al)", Location = new Point(20, 380), Width = 380, Height = 45, BackColor = Color.FromArgb(33, 115, 70), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnExport.Click += BtnExport_Click;

            var btnImport = new Button { Text = "EXCEL'DEN YÜKLE (Güncelle)", Location = new Point(20, 435), Width = 380, Height = 45, BackColor = Color.DarkOrange, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            btnImport.Click += BtnImport_Click;

            panelLeft.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDescription, btnSave, btnDelete, btnClose, btnExport, btnImport });

            // Grid
            dgvCategories = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = ThemeManager.BackgroundColor
            };
            dgvCategories.CellDoubleClick += DgvCategories_CellDoubleClick;
            this.Controls.Add(dgvCategories);
            dgvCategories.BringToFront();
        }

        private void LoadCategories()
        {
            using (var db = new MarketDbContext())
            {
                dgvCategories.DataSource = db.Categories.Select(c => new { c.Id, c.Name, c.Description }).ToList();
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) return;

            using (var db = new MarketDbContext())
            {
                if (selectedCategoryId == 0)
                {
                    db.Categories.Add(new Category { Name = txtName.Text, Description = txtDescription.Text });
                    Logger.LogActivity("CATEGORY_ADD", $"{txtName.Text} eklendi.");
                }
                else
                {
                    var cat = db.Categories.Find(selectedCategoryId);
                    if (cat != null)
                    {
                        cat.Name = txtName.Text;
                        cat.Description = txtDescription.Text;
                        Logger.LogActivity("CATEGORY_UPDATE", $"{txtName.Text} güncellendi.");
                    }
                }
                db.SaveChanges();
            }
            ClearForm();
            LoadCategories();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (selectedCategoryId == 0) return;
            if (MessageBox.Show("Silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using (var db = new MarketDbContext())
                {
                    var cat = db.Categories.Find(selectedCategoryId);
                    if (cat != null)
                    {
                        db.Categories.Remove(cat);
                        db.SaveChanges();
                        Logger.LogActivity("CATEGORY_DELETE", $"{cat.Name} silindi.");
                    }
                }
                ClearForm();
                LoadCategories();
            }
        }

        private void DgvCategories_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedCategoryId = Convert.ToInt32(dgvCategories.Rows[e.RowIndex].Cells["Id"].Value);
                txtName.Text = dgvCategories.Rows[e.RowIndex].Cells["Name"].Value?.ToString();
                txtDescription.Text = dgvCategories.Rows[e.RowIndex].Cells["Description"].Value?.ToString();
            }
        }

        private void ClearForm()
        {
            selectedCategoryId = 0;
            txtName.Clear();
            txtDescription.Clear();
        }

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Çalışma Kitabı|*.xlsx", FileName = "Kategoriler_" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Kategoriler");
                            ws.Cell(1, 1).Value = "Kategori Adı";
                            ws.Cell(1, 2).Value = "Açıklama";

                            var header = ws.Range(1, 1, 1, 2);
                            header.Style.Font.Bold = true;
                            header.Style.Fill.BackgroundColor = XLColor.AirForceBlue;
                            header.Style.Font.FontColor = XLColor.White;

                            using (var db = new MarketDbContext())
                            {
                                var categories = db.Categories.ToList();
                                int row = 2;
                                foreach (var c in categories)
                                {
                                    ws.Cell(row, 1).Value = c.Name;
                                    ws.Cell(row, 2).Value = c.Description;
                                    row++;
                                }
                            }
                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            MessageBox.Show("Kategoriler başarıyla Excel'e aktarıldı!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            if (MessageBox.Show("Kategori Adı eşleşenlerin açıklaması güncellenecek, eşleşmeyenler yeni kategori olarak eklenecektir.\nDevam edilsin mi?", "Toplu Güncelleme Uyarısı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
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
                                string catName = row.Cell(1).Value.ToString().Trim();
                                if (string.IsNullOrEmpty(catName)) continue;

                                string desc = row.Cell(2).Value.ToString().Trim();

                                var existing = db.Categories.FirstOrDefault(c => c.Name == catName);
                                if (existing != null)
                                {
                                    existing.Description = desc;
                                    updatedCount++;
                                }
                                else
                                {
                                    db.Categories.Add(new Category 
                                    {
                                        Name = catName,
                                        Description = desc
                                    });
                                    addedCount++;
                                }
                            }

                            db.SaveChanges();
                            Logger.LogActivity("EXCEL_SYNC", $"Kategori Excel senk: {updatedCount} güncellendi, {addedCount} eklendi.");
                            MessageBox.Show($"Senkronizasyon Tamamlandı!\nGüncellenen: {updatedCount}\nYeni Eklenen: {addedCount}", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadCategories();
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
