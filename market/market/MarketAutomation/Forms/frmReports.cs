using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MarketAutomation.Data;
using MarketAutomation.Helpers;

namespace MarketAutomation.Forms
{
    public class frmReports : Form
    {
        private TextBox txtReportOutput;
        private Button btnGenerate, btnCloseDay, btnClose;
        private DateTimePicker dtpDate;

        public frmReports()
        {
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
        }

        private void InitializeComponent()
        {
            this.Size = new Size(950, 750);
            this.Text = "Raporlar ve Gün Sonu";

            var panelTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = ThemeManager.SurfaceColor, Padding = new Padding(20) };
            this.Controls.Add(panelTop);

            var lblTitle = new Label { Text = "GÜN SONU (Z) RAPORU VE ANALİZ", Font = ThemeManager.HeaderFont, AutoSize = true, Location = new Point(20, 10) };
            panelTop.Controls.Add(lblTitle);

            panelTop.Controls.Add(new Label { Text = "Rapor Tarihi:", AutoSize = true, Location = new Point(20, 60) });
            dtpDate = new DateTimePicker { Location = new Point(160, 55), Width = 200, Format = DateTimePickerFormat.Short };
            panelTop.Controls.Add(dtpDate);

            btnGenerate = new Button { Text = "RAPORU OLUŞTUR", Location = new Point(400, 50), Width = 200, Height = 35, BackColor = ThemeManager.PrimaryColor, ForeColor = Color.White };
            btnGenerate.Click += BtnGenerate_Click;
            panelTop.Controls.Add(btnGenerate);

            btnCloseDay = new Button { Text = "GÜNÜ KAPAT VE MAİL AT", Location = new Point(650, 50), Width = 250, Height = 35, BackColor = ThemeManager.SuccessColor, ForeColor = Color.White };
            btnCloseDay.Click += BtnCloseDay_Click;
            panelTop.Controls.Add(btnCloseDay);

            txtReportOutput = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Font = new Font("Courier New", 14),
                ReadOnly = true,
                BackColor = Color.Black,
                ForeColor = Color.Lime
            };
            this.Controls.Add(txtReportOutput);
            txtReportOutput.BringToFront();

            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = ThemeManager.SurfaceColor };
            btnClose = new Button { Text = "KAPAT", Location = new Point(20, 10), Width = 250, Height = 40, BackColor = ThemeManager.SecondaryColor, ForeColor = Color.White };
            btnClose.Click += (s, e) => this.Close();
            panelBottom.Controls.Add(btnClose);
            this.Controls.Add(panelBottom);
        }

        private void BtnGenerate_Click(object? sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            DateTime selectedDate = dtpDate.Value.Date;
            StringBuilder sb = new StringBuilder();

            using (var db = new MarketDbContext())
            {
                var sales = db.Sales.Where(s => s.SaleDate.Date == selectedDate).ToList();
                var logs = db.ActivityLogs.Where(l => l.LogDate.Date == selectedDate && 
                    (l.ActionType.Contains("CANCEL") || l.ActionType.Contains("DELETE") || l.ActionType.Contains("INVOICE"))).ToList();

                decimal totalCash = sales.Where(s => s.PaymentMethod == "Nakit").Sum(s => s.CashPaid);
                decimal totalCard = sales.Where(s => s.PaymentMethod == "Kredi Kartı").Sum(s => s.CardPaid);
                decimal totalCredit = sales.Where(s => s.PaymentMethod == "Veresiye").Sum(s => s.GrandTotal);
                decimal totalTahsilat = sales.Where(s => s.PaymentMethod == "Tahsilat").Sum(s => s.CashPaid);

                decimal totalDiscount = sales.Sum(s => s.DiscountAmount);
                decimal grandTotal = sales.Sum(s => s.GrandTotal);

                sb.AppendLine("======================================================");
                sb.AppendLine($"              Z RAPORU - {selectedDate.ToShortDateString()}");
                sb.AppendLine("======================================================");
                sb.AppendLine($"Toplam Satış Fişi Sayısı : {sales.Count(s => s.PaymentMethod != "Tahsilat")}");
                sb.AppendLine($"Toplam İndirimler        : {totalDiscount:C2}");
                sb.AppendLine("------------------------------------------------------");
                sb.AppendLine($"SATIŞ (NAKİT)            : {totalCash:C2}");
                sb.AppendLine($"SATIŞ (KREDİ KARTI)      : {totalCard:C2}");
                sb.AppendLine($"SATIŞ (VERESİYE YAZILAN) : {totalCredit:C2}");
                sb.AppendLine("------------------------------------------------------");
                sb.AppendLine($"GÜNLÜK TOPLAM CİRO     : {grandTotal:C2}");
                sb.AppendLine("======================================================");
                sb.AppendLine($"ESKİ BORÇLARDAN NAKİT TAHSİLAT: + {totalTahsilat:C2}");
                sb.AppendLine($"KASADAKİ NET NAKİT (Nakit Satış + Tahsilat): {(totalCash + totalTahsilat):C2}");
                sb.AppendLine("======================================================");
                sb.AppendLine("KASADAKİ İPTAL VE MAĞAZA İŞLEMLERİ (ŞÜPHELİ HAREKETLER)");
                sb.AppendLine("------------------------------------------------------");

                if (!logs.Any())
                {
                    sb.AppendLine("Kayıtlı işlem bulunmadı.");
                }
                else
                {
                    foreach (var log in logs)
                    {
                        sb.AppendLine($"[{log.LogDate.ToShortTimeString()}] - {log.ActionType}: {log.Description}");
                    }
                }
                sb.AppendLine("======================================================");

                var topProducts = db.SaleDetails
                                    .Where(sd => sd.Sale.SaleDate.Date == selectedDate)
                                    .GroupBy(sd => sd.Product.Name)
                                    .Select(g => new { Name = g.Key, Qty = g.Sum(x => x.Quantity) })
                                    .OrderByDescending(x => x.Qty)
                                    .Take(5)
                                    .ToList();

                sb.AppendLine("EN ÇOK SATAN İLK 5 ÜRÜN");
                sb.AppendLine("------------------------------------------------------");
                foreach(var p in topProducts)
                {
                    sb.AppendLine($"- {p.Name} : {p.Qty} adet/kg");
                }
                sb.AppendLine("======================================================");
                sb.AppendLine("             RAPOR SONU              ");
            }

            txtReportOutput.Text = sb.ToString();
            txtReportOutput.SelectionStart = 0;
            txtReportOutput.ScrollToCaret();
        }

        private void BtnCloseDay_Click(object? sender, EventArgs e)
        {
            GenerateReport();
            if (MessageBox.Show("Günün satışlarını kapatıp Z Raporunu yöneticiye e-posta olarak göndermek istediğinize emin misiniz?", "Gün Sonu", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                using (var db = new MarketDbContext())
                {
                    DateTime selectedDate = dtpDate.Value.Date;
                    var sales = db.Sales.Where(s => s.SaleDate.Date == selectedDate).ToList();
                    
                    decimal totalCash = sales.Where(s => s.PaymentMethod == "Nakit").Sum(s => s.CashPaid);
                    decimal totalTahsilat = sales.Where(s => s.PaymentMethod == "Tahsilat").Sum(s => s.CashPaid);
                    decimal netCash = totalCash + totalTahsilat;
                    decimal totalCard = sales.Where(s => s.PaymentMethod == "Kredi Kartı").Sum(s => s.CardPaid);

                    bool isSent = EmailHelper.SendZReportEmail(txtReportOutput.Text, netCash, totalCard);
                    if (isSent)
                    {
                        Logger.LogActivity("Z_REPORT_MAILED", $"{selectedDate.ToShortDateString()} Z Raporu alındı ve mail atıldı.");
                        MessageBox.Show("Rapor başarıyla gönderildi ve Z raporu kapatıldı.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Email gönderilirken teknik bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
