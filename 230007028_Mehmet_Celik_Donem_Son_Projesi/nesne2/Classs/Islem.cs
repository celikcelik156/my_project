using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nesne2.Classs
{
    public class AtikIslem
    {
        public int IslemID { get; set; }
        public int TesisID { get; set; }
        public int AtikID { get; set; }
        public int AtikIslemID { get; set; }
        public decimal Miktar { get; set; }
        public DateTime IslemTarihi { get; set; }
        public decimal ToplamTutar { get; set; }
        public GeriDonusumTesisi Tesis { get; set; }
        public AtikTuru AtikTuru { get; set; }
    }


    public class Bakim
    {
        public int BakimID { get; set; }
        public int BakimTurID { get; set; }
        public int AracSasiID { get; set; }
        public DateTime BakimTarihi { get; set; }

        public BakimTur BakimTuru { get; set; }
        public Arac YapilanArac { get; set; }
    }
}
