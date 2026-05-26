using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace WebProgramlamaProjesi_MehmetCelik.Models
{
    public class Slider
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık gereklidir")]
        [StringLength(200)]
        public string Baslik { get; set; }

        [StringLength(500)]
        public string Aciklama { get; set; }

        [Required(ErrorMessage = "Resim yolu gereklidir")]
        [StringLength(500)]
        public string ResimYolu { get; set; }

        [NotMapped]
        public HttpPostedFileBase ResimDosyasi { get; set; }

        [StringLength(200)]
        public string Link { get; set; }

        [StringLength(50)]
        public string LinkMetni { get; set; }

        public int Sira { get; set; }

        public bool Aktif { get; set; }
    }
}

