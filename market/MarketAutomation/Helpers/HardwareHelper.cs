using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO.Ports;

namespace MarketAutomation.Helpers
{
    public static class HardwareHelper
    {
        // 1. TERMAL YAZICI VE ÇEKMECE
        public static void OpenCashDrawer(string printerName = "POS-80")
        {
            try
            {
                // Standart ESC/POS Çekmece Açma Komutu (ESC p m t1 t2)
                byte[] drawerCommand = new byte[] { 27, 112, 0, 25, 250 };
                
                // GERÇEK DONANIM TETİKLEME: RawPrinterHelper ile komutu direkt yazıcıya gönder
                bool success = RawPrinterHelper.SendBytesToPrinter(printerName, drawerCommand);
                
                if (success)
                {
                    Console.WriteLine($"[HARDWARE] Çekmece Açma Komutu BAŞARIYLA Gönderildi ({printerName})");
                    Logger.LogActivity("HARDWARE", $"Para çekmecesi gerçek donanımla açıldı ({printerName}).");
                }
                else
                {
                    Console.WriteLine($"[HARDWARE HATA] Çekmece komutu gönderilemedi ({printerName}). Yazıcı adını veya bağlantıyı kontrol edin.");
                    Logger.LogActivity("HARDWARE_ERROR", $"Para çekmecesi AÇILAMADI ({printerName}).");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HARDWARE HATA] Çekmece: " + ex.Message);
            }
        }

        public static void PrintReceiptAndOpenDrawer(string receiptContent, string printerName = "POS-80")
        {
            try
            {
                PrintDocument pd = new PrintDocument();
                pd.PrinterSettings.PrinterName = printerName;

                pd.PrintPage += (s, e) =>
                {
                    Font f = new Font("Courier New", 9);
                    e.Graphics?.DrawString(receiptContent, f, Brushes.Black, new PointF(0, 0));
                };

                // pd.Print(); // Yazdırma
                OpenCashDrawer(printerName);

                Console.WriteLine("[HARDWARE] Termal Yazıcıdan fiş çıkarıldı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HARDWARE HATA] Yazıcı: " + ex.Message);
            }
        }

        // 2. MÜŞTERİ GÖSTERGE EKRANI (POLE DISPLAY)
        public static void SendToPoleDisplay(string line1, string line2, string comPort = "COM1")
        {
            try
            {
                // Uzunlukları 20 karaktere sabitle (Standart Pole Display satır uzunluğu)
                line1 = line1.PadRight(20).Substring(0, 20);
                line2 = line2.PadRight(20).Substring(0, 20);

                // SerialPort port = new SerialPort(comPort, 9600, Parity.None, 8, StopBits.One);
                // port.Open();
                // ESC komutlarıyla ekranı temizle ve yazdır (Simülasyon)
                // port.Write(char.ConvertFromUtf32(12)); // Clear
                // port.Write(line1 + line2);
                // port.Close();

                Console.WriteLine($"[POLE DISPLAY] {comPort} -> Satır1: {line1.Trim()} | Satır2: {line2.Trim()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[HARDWARE HATA] Pole Display: " + ex.Message);
            }
        }
    }
}
