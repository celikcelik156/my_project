using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nesne2.Classs
{
    public class Personel
    {
        public int TC { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public int Yas { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string DogumYeri { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Sifre { get; set; }
        public bool IsAdmin { get; set; } 
        public bool DepartmanYoneticisi { get; set; }
        public string Adres { get; set; }
        public int  CocukSayisi { get; set; }
        public int UnvanID { get; set; }
        public decimal Maas { get; set; }
        public bool MaasYattiMi { get; set; }
        public int? AracSasiID { get; set; }

        public Unvan Unvan { get; set; }
        public Arac ZimmetliArac { get; set; }

        public string TamAd
        {
            get { return Ad + " " + Soyad; }
        }
        public int maasHesapla(int maas)
        {
            int ekstraMaas = (CocukSayisi * 400); 
            return maas + ekstraMaas;
        }
    }
}
