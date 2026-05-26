using System;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MarketAutomation.Helpers
{
    public class HztsPosService
    {
        private string _portName;
        private const byte STX = 0x02;
        private const byte ETX = 0x03;

        public HztsPosService(string portName = "COM3")
        {
            _portName = portName;
        }

        public async Task<(bool Success, string Message)> ProcessPaymentAsync(decimal amount)
        {
            try
            {
                // Tutarı 12 haneli kuruş formatına çevir (Örn: 10.50 -> 000000001050)
                long kurus = (long)(amount * 100);
                string amountStr = kurus.ToString().PadLeft(12, '0');

                // Mesaj Paketi Hazırla: ID(2 byte) + MsgType(1 byte: 01=Sale) + Amount(12 byte)
                string id = "01"; 
                string msgType = "01"; // Satış
                string body = id + msgType + amountStr;
                
                byte[] packet = BuildPacket(body);

                using (SerialPort port = new SerialPort(_portName, 9600, Parity.None, 8, StopBits.One))
                {
                    port.ReadTimeout = 60000; // 60 saniye bekleme (POS işlemi uzun sürebilir)
                    port.WriteTimeout = 5000;
                    
                    try { port.Open(); }
                    catch (Exception ex) { return (false, "POS Cihazına bağlanılamadı: " + ex.Message); }

                    // Paketi gönder
                    port.Write(packet, 0, packet.Length);

                    // Cevap bekle (Basitleştirilmiş HZTS ACK/NAK ve Data çevrimi)
                    // Gerçek uygulamada State Machine kullanılır, burada asenkron simülasyon/okuma yapıyoruz
                    byte[] response = await ReadResponseAsync(port);

                    if (response == null || response.Length == 0)
                        return (false, "POS Cihazından cevap alınamadı (Timeout).");

                    // Cevap analizi (Örn: 06 = ACK, 15 = NAK, veya data paketi)
                    if (response[0] == 0x06) // ACK
                    {
                        // ACK sonrası asıl sonuç paketini bekle
                        byte[] resultPacket = await ReadResponseAsync(port);
                        if (resultPacket != null && resultPacket.Contains((byte)'0') && resultPacket.Contains((byte)'0')) // "00" Genelde başarıdır
                        {
                            return (true, "Ödeme Onaylandı.");
                        }
                    }

                    return (false, "Ödeme Reddedildi veya İşlem İptal Edildi.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Hata: " + ex.Message);
            }
        }

        private byte[] BuildPacket(string body)
        {
            byte[] bodyBytes = Encoding.ASCII.GetBytes(body);
            int len = bodyBytes.Length;
            
            // STX + LEN(2) + BODY + ETX + LRC
            byte[] packet = new byte[1 + 2 + len + 1 + 1];
            packet[0] = STX;
            packet[1] = (byte)(len >> 8);
            packet[2] = (byte)(len & 0xFF);
            Array.Copy(bodyBytes, 0, packet, 3, len);
            packet[3 + len] = ETX;
            
            packet[4 + len] = CalculateLrc(packet, 1, 3 + len); // LEN'den ETX'e kadar
            
            return packet;
        }

        private byte CalculateLrc(byte[] data, int start, int end)
        {
            byte lrc = 0;
            for (int i = start; i <= end; i++)
                lrc ^= data[i];
            return lrc;
        }

        private async Task<byte[]> ReadResponseAsync(SerialPort port)
        {
            return await Task.Run(() =>
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = port.Read(buffer, 0, buffer.Length);
                    byte[] result = new byte[bytesRead];
                    Array.Copy(buffer, result, bytesRead);
                    return result;
                }
                catch { return null; }
            });
        }
    }
}
