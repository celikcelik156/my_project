using System;

namespace MarketAutomation.Helpers
{
    public static class SmsHelper
    {
        public static bool SendSms(string phoneNumber, string message)
        {
            // Gerçek bir senaryoda burada Netgsm, Mutlucell gibi firmaların HTTP API istekleri yapılır
            // HttpClient client = new HttpClient();
            // ... API post işlemleri ...

            Console.WriteLine($"[SMS GÖNDERİLDİ] Tel: {phoneNumber} | Mesaj: {message}");
            return true; // Simülasyon
        }
    }
}
