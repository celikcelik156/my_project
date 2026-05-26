using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nesne2.Classs
{
    public class Arac
    {
        public int AracSasiID { get; set; }
        public string Marka { get; set; }
        public string Model { get; set; }
        public decimal Kapasite { get; set; }
        public string Plaka { get; set; }
        public int AracTipiID { get; set; }
        public int RotaID { get; set; }
        public AracTipi AracTipi { get; set; }
        public Rota Rota { get; set; }
        public Durum Durum { get; set; }


        public bool AktifMi
        {
            get
            {
                return Durum != null && Durum.DurumAdi == "Aktif";
            }
        }

        public string AracDurum(string durum)
        {
            switch (durum)
            {
                case "Aktif":
                    return "1";
                case "Pasif":
                    return "2";
                case "Hasarlı":
                    return "3";
                case "Bakımda":
                    return "4";
                case "Görevde":
                    return "5";
                default:
                    return "Bilinmiyor";
            }
        }
    }
}
