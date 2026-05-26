using System.ComponentModel.DataAnnotations;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public class Hizmet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir")]
        [StringLength(200)]
        public string Baslik { get; set; }

        [StringLength(500)]
        public string Aciklama { get; set; }

        [StringLength(100)]
        public string Icon { get; set; }

        public int Sira { get; set; }

        public bool Aktif { get; set; }
    }
}

