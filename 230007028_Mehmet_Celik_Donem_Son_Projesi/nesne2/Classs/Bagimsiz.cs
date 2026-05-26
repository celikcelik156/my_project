using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nesne2.Classs
{

    public class Rota
    {
        public int RotaID { get; set; }
        public string RotaAdi { get; set; }

        public override string ToString()
        {
            return RotaAdi;
        }
    }

    public class AracTipi
    {
        public int AracTipiID { get; set; }
        public string TipAdi { get; set; }
    }

    public class Durum
    {
        public int DurumID { get; set; }
        public string DurumAdi { get; set; }
    }

    public class AtikTuru
    {
        public int AtikID { get; set; }
        public string AtikAdi { get; set; }
        public decimal BirimFiyat { get; set; }
    }

    public class GeriDonusumTesisi
    {
        public int TesisID { get; set; }
        public string TesisAdi { get; set; }
        public decimal Kapasite { get; set; }
    }

    public class Unvan
    {
        public int UnvanID { get; set; }
        public string UnvanAdi { get; set; }
        public decimal MaasMin { get; set; }
        public decimal MaasMax { get; set; }
    }

    public class BakimTur
    {
        public int BakimTurID { get; set; }
        public string BakimAdi { get; set; }
        public decimal BakimFiyati { get; set; }
    }
}