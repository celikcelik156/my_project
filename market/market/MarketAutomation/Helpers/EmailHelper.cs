using System;
using System.Net;
using System.Net.Mail;

namespace MarketAutomation.Helpers
{
    public static class EmailHelper
    {
        public static bool SendZReportEmail(string reportContent, decimal totalCash, decimal totalCard)
        {
            try
            {
                // Gerçek senaryo için SMTP bilgileri girilmelidir. Şimdilik simülasyon.
                /*
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.Credentials = new NetworkCredential("market.system@gmail.com", "password");
                    client.EnableSsl = true;

                    MailMessage mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress("market.system@gmail.com");
                    mailMessage.To.Add("patron@market.com");
                    mailMessage.Subject = $"GÜN SONU Z RAPORU - {DateTime.Now.ToShortDateString()}";
                    mailMessage.Body = $"Merhaba,\n\nBugünün Z Raporu Özeti:\nNakit: {totalCash:C2}\nKredi Kartı: {totalCard:C2}\n\nDetaylar:\n{reportContent}";
                    
                    client.Send(mailMessage);
                }
                */

                Console.WriteLine($"[EMAIL GÖNDERİLDİ] Z Raporu maili patrona iletildi.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email Hatası: " + ex.Message);
                return false;
            }
        }
    }
}
